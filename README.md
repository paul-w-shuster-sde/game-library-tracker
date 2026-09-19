### Game Library Analytics & Validation Engine

An enterprise-grade, backend data pipeline and validation engine engineered in **.NET 8 (C#)** designed to ingest, sanitize, and normalize distributed gaming inventory streams into a relational **PostgreSQL** data warehouse. 

This portfolio project demonstrates a rigorous approach to handling backend data pipelines, relational query optimizations, data hygiene enforcement, and cross-platform automation. 

### 🏗️ Architectural Topology

The system is split into distinct operational layers to ensure absolute separation of concerns: 

1. **Client Processing Layer (C# / .NET 8):** Processes incoming raw flat-file streams (CSV datasets), utilizing custom extraction objects to map raw entries into typed memory representations.
2. **Data Integrity & Rules Engine (DataLedgerValidationEngine.cs):** Evaluates incoming records against business logic rules before they touch the database layer, focusing on invariant validation and low-allocation performance.
3. **Data Infrastructure Layer (PostgreSQL):** A robust relational database schema optimized with strict constraint mappings, cascaded relational rules, and performance indexes for analytical querying.
4. **DevOps & Migration Automation (Jupyter Notebook / Python):** Operates as an environmental migration supervisor utilizing SQLAlchemy to establish automated DDL deployment, database teardown/rebuild, and post-migration validation checks.

### 📊 Relational Database Design & Optimizations (schema.sql)

The underlying storage engine rejects packed arrays and unstructured blobs in favor of a strictly normalized Third Normal Form (3NF) relational layout: 

* **Core Tables:** platforms, games, and genres enforce exact domain definition constraints.
* **Many-to-Many Mapping:** The games_genres intersection table normalizes descriptive entity metadata without introducing spatial redundancies.
* **SARGable Query Indexes:** Specialized B-Tree indexes are explicitly applied to high-frequency query filters (completion_status, hours_played) to guarantee optimized search index seek operations.
* **Pre-Aggregated Read Subsystems:** Pre-compiled database views offload runtime computational overhead from application memory down to the database engine: 

  * view_active_games: Instantly routes processing logic straight to active operational records.
  * view_backlog_games: Flattens complex, many-to-many hierarchical records using relational aggregation (STRING_AGG), providing instant read metrics for backlog analysis tools.

### 🛠️ Data Integrity & Pipeline Hygiene

To prevent data corruption, the system handles data constraints both in application code and at the database boundary: 

* **Strong Typing & Enums:** Rejects arbitrary input fields by binding operational states straight to strict domain conditions (Active, Backlog, Completed, Abandoned).
* **Relational Safety Margins:** Implements ON DELETE CASCADE on link mappings and ON DELETE RESTRICT on core hardware platform identifiers to guarantee zero orphaned records.
* **Mathematical Invariants:** Employs explicit database-level CHECK constraints to ensure metrics such as price entries and accumulated operational hours can never drop below zero.

### 🚀 Workstation Orchestration & Setup

### 1. Prerequisites

* **Runtime:** .NET 8.0 SDK
* **Database Platform:** PostgreSQL 16+ (Remote Linux or local instance)
* **Python Stack (For Migrations):** Python 3.10+, sqlalchemy, psycopg2

### 2. Environmental Initialization

The Python migration suite reads core environmental variables from the system path, ensuring configuration files remain completely free of hardcoded security parameters: 

bash

export DB_HOST="your-linux-database-ip"
export DB_PORT="5432"
export DB_NAME="game_library"
export DB_USER="postgres"
export DB_PASS="your-secure-password"

Use code with caution.

### 3. Executing Database Migrations

Run the accompanying Jupyter Notebook (migration_pipeline.ipynb) to automatically verify connectivity, clear stale assets, compile your relational layout from schema.sql, and execute a post-deployment verification scan against the system catalog. 

bash

jupyter notebook migration_pipeline.ipynb

Use code with caution.

### 4. Running the C# Application Pipeline

Compile and execute the core .NET data processing pipeline from your workstation terminal: 

powershell

dotnet restore
dotnet build --configuration Release
dotnet run --project GameLibraryAnalytics.csproj

Use code with caution.