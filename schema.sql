-- ============================================================================
-- PROJECT: Game Library Analytics & Validation Engine
-- COMPONENT: PostgreSQL DDL Database Schema Migration Blueprint
-- AUTHOR: Paul W. Shuster
-- DESCRIPTION: Recreates normalized entity tables, primary/foreign key relations,
--              cascade constraints, and pre-aggregated analytical views.
-- ============================================================================

-- Drop dependent assets cleanly if resetting the environment
DROP VIEW IF EXISTS view_backlog_games;
DROP VIEW IF EXISTS view_active_games;
DROP TABLE IF EXISTS games_genres cascade;
DROP TABLE IF EXISTS genres cascade;
DROP TABLE IF EXISTS games cascade;
DROP TABLE IF EXISTS platforms cascade;

-- ----------------------------------------------------------------------------
-- 1. ENTITY CORE TABLES
-- ----------------------------------------------------------------------------

-- Target Table: Platforms (Stores explicit operating frameworks/hardware targets)
CREATE TABLE platforms (
    platform_id SERIAL PRIMARY KEY,
    platform_name VARCHAR(100) NOT NULL UNIQUE
);

-- Target Table: Games (Central transaction and metric storage entity)
CREATE TABLE games (
    game_id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    platform_id INT NOT NULL REFERENCES platforms(platform_id) ON DELETE RESTRICT,
    hours_played NUMERIC(10, 2) DEFAULT 0.00 CHECK (hours_played >= 0.00),
    purchase_price NUMERIC(10, 2) DEFAULT 0.00 CHECK (purchase_price >= 0.00),
    completion_status VARCHAR(50) DEFAULT 'Backlog' CHECK (completion_status IN ('Active', 'Backlog', 'Completed', 'Abandoned')),
    release_date VARCHAR(50) NULL,
    CONSTRAINT unique_title_platform UNIQUE (title, platform_id)
);

-- Target Table: Genres (Normalized lookup catalog for categorizing libraries)
CREATE TABLE genres (
    genre_id SERIAL PRIMARY KEY,
    genre_name VARCHAR(100) NOT NULL UNIQUE
);

-- ----------------------------------------------------------------------------
-- 2. RELATIONAL MAPPING INTERSECT TABLES (Many-to-Many Relationships)
-- ----------------------------------------------------------------------------
CREATE TABLE games_genres (
    game_id INT REFERENCES games(game_id) ON DELETE CASCADE,
    genre_id INT REFERENCES genres(genre_id) ON DELETE CASCADE,
    PRIMARY KEY (game_id, genre_id)
);

-- ----------------------------------------------------------------------------
-- 3. PERFORMANCE INDEX OPTIMIZATIONS (Ensures SARGable read path optimizations)
-- ----------------------------------------------------------------------------
CREATE INDEX idx_games_completion_status ON games(completion_status);
CREATE INDEX idx_games_hours_played ON games(hours_played);

-- ----------------------------------------------------------------------------
-- 4. ANALYTICAL DATABASE VIEWS (Pre-aggregated logical abstractions)
-- ----------------------------------------------------------------------------

-- View: Active Game Queue
-- Purpose: Filters raw inventory datasets down to active execution tracks
CREATE OR REPLACE VIEW view_active_games AS
SELECT 
    g.game_id, 
    g.title, 
    p.platform_name,
    g.hours_played, 
    g.completion_status
FROM games g
JOIN platforms p ON g.platform_id = p.platform_id
WHERE g.completion_status = 'Active';

-- View: Backlog Optimization Engine Matrix
-- Purpose: Aggregates descriptive catalog features and flattens hierarchy fields
CREATE OR REPLACE VIEW view_backlog_games AS
SELECT 
    g.game_id, 
    g.title, 
    p.platform_name,
    g.hours_played,
    g.purchase_price,
    g.completion_status, 
    COALESCE(STRING_AGG(gn.genre_name, ', ' ORDER BY gn.genre_name), 'Unassigned') AS combined_genres
FROM games g
JOIN platforms p ON g.platform_id = p.platform_id
LEFT JOIN games_genres gg ON g.game_id = gg.game_id
LEFT JOIN genres gn ON gg.genre_id = gn.genre_id
WHERE g.completion_status = 'Backlog'
GROUP BY g.game_id, g.title, p.platform_name, g.hours_played, g.purchase_price, g.completion_status;
