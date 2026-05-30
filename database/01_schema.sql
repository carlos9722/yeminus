-- Service Orders - esquema inicial (PostgreSQL)
-- Ejecutado automáticamente al levantar el contenedor por primera vez.

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Usuarios (autenticación JWT)
CREATE TABLE users (
    id              SERIAL PRIMARY KEY,
    username        VARCHAR(50)  NOT NULL UNIQUE,
    password_hash   VARCHAR(255) NOT NULL,
    full_name       VARCHAR(150) NOT NULL,
    is_active       BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

-- Clientes (soft delete con deleted_at)
CREATE TABLE clients (
    id              SERIAL PRIMARY KEY,
    full_name       VARCHAR(150) NOT NULL,
    identity_doc    VARCHAR(30)  NOT NULL,
    address         VARCHAR(250) NOT NULL,
    phone           VARCHAR(20)  NOT NULL,
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    deleted_at      TIMESTAMPTZ  NULL
);

-- Documento único solo entre clientes activos (no eliminados)
CREATE UNIQUE INDEX idx_clients_identity_doc_active
    ON clients (identity_doc)
    WHERE deleted_at IS NULL;

-- Técnicos (soft delete)
CREATE TABLE technicians (
    id              SERIAL PRIMARY KEY,
    full_name       VARCHAR(150) NOT NULL,
    phone           VARCHAR(20)  NOT NULL,
    specialty       VARCHAR(100) NOT NULL,
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    deleted_at      TIMESTAMPTZ  NULL
);

-- Estados de orden (catálogo fijo)
CREATE TYPE order_status AS ENUM ('Pendiente', 'EnProgreso', 'Finalizada');

-- Órdenes de servicio (soft delete)
CREATE TABLE service_orders (
    id              SERIAL PRIMARY KEY,
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    status          order_status NOT NULL DEFAULT 'Pendiente',
    description     TEXT         NOT NULL,
    technician_id   INT          NOT NULL REFERENCES technicians(id),
    client_id       INT          NOT NULL REFERENCES clients(id),
    updated_at      TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    deleted_at      TIMESTAMPTZ  NULL
);

-- Índices para filtros del listado
CREATE INDEX idx_service_orders_status ON service_orders (status);
CREATE INDEX idx_service_orders_created_at ON service_orders (created_at);
CREATE INDEX idx_service_orders_technician_id ON service_orders (technician_id);
CREATE INDEX idx_service_orders_client_id ON service_orders (client_id);
CREATE INDEX idx_technicians_specialty ON technicians (specialty) WHERE deleted_at IS NULL;
CREATE INDEX idx_technicians_deleted_at ON technicians (deleted_at);
CREATE INDEX idx_clients_deleted_at ON clients (deleted_at);
CREATE INDEX idx_service_orders_deleted_at ON service_orders (deleted_at);

-- Usuario precargado: admin / Admin123!
-- Hash BCrypt generado en BD (compatible con BCrypt.Net-Next en .NET)
INSERT INTO users (username, password_hash, full_name)
VALUES (
    'admin',
    crypt('Admin123!', gen_salt('bf', 11)),
    'Administrador del Sistema'
);
