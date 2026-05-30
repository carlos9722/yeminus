
BEGIN;

TRUNCATE TABLE service_orders RESTART IDENTITY CASCADE;
TRUNCATE TABLE technicians RESTART IDENTITY CASCADE;
TRUNCATE TABLE clients RESTART IDENTITY CASCADE;

INSERT INTO clients (full_name, identity_doc, address, phone) VALUES
    ('María García López',       '1075308563', 'Calle 10 # 25-30, Bogotá',     '+57 3001234567'),
    ('Carlos Enrique Ruiz',      '80123456',   'Av. Suba 95-20, Bogotá',       '+57 3109876543'),
    ('Ana Patricia Muñoz',       '52987654',   'Carrera 7 # 45-12, Medellín',  '+57 3205551212'),
    ('Jorge Iván Castillo',      '9456123780', 'Transversal 50 # 8-90, Cali',  '+57 3154448899'),
    ('Lucía Fernanda Ortiz',     '1122334455', 'Calle 100 # 19-05, Bogotá',    '+57 3017776655');

INSERT INTO technicians (full_name, phone, specialty) VALUES
    ('Pedro Ramírez',      '+57 3001112233', 'Electricidad'),
    ('Sandra Mejía',       '+57 3112223344', 'Electricidad'),
    ('Luis Alberto Díaz',  '+57 3203334455', 'Plomería'),
    ('Diana Torres',       '+57 3154445566', 'Aire acondicionado'),
    ('Miguel Ángel Soto',  '+57 3185556677', 'Gas');

INSERT INTO service_orders (created_at, status, description, technician_id, client_id) VALUES
    ('2026-05-25 14:00:00+00', 'Pendiente',   'Inspección eléctrica apartamento Ana Patricia.',           1, 3),
    ('2026-05-23 10:00:00+00', 'Pendiente',   'Revisión de tablero eléctrico en cocina.',               1, 1),
    ('2026-05-22 11:00:00+00', 'EnProgreso',  'Instalación de puntos de luz en sala y comedor.',        2, 2),
    ('2026-05-27 09:00:00+00', 'EnProgreso',  'Cambio de breaker en zona de lavandería.',               1, 3),
    ('2026-05-15 08:00:00+00', 'Finalizada',  'Reparación de fuga en baño principal.',                  3, 4),
    ('2026-05-18 16:00:00+00', 'Finalizada',  'Mantenimiento preventivo red eléctrica.',                  2, 1),
    ('2026-05-24 13:00:00+00', 'Pendiente',   'Instalación de calentador a gas.',                       5, 2),
    ('2026-05-26 15:00:00+00', 'EnProgreso',  'Limpieza y carga de refrigerante split.',                4, 5);

INSERT INTO clients (full_name, identity_doc, address, phone, deleted_at) VALUES
    ('Cliente inactivo prueba', '9999999999', 'Dirección antigua', '+57 3000000000', NOW());

INSERT INTO technicians (full_name, phone, specialty, deleted_at) VALUES
    ('Técnico inactivo prueba', '+57 3000000001', 'Electricidad', NOW());

COMMIT;
