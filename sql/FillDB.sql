USE LibraryDb;
GO

SET NOCOUNT ON;

INSERT INTO Role (name, description) VALUES
    (N'Библиотекарь абонемента',             N'Обслуживание читателей: выдача, возврат, продление, задолженности'),
    (N'Администрация библиотеки',            N'Управление пользователями, справочниками, отчётность, резервное копирование'),
    (N'Сотрудник отдела комплектования',     N'Поступления и списания экземпляров, ведение изданий и авторов');

INSERT INTO [User] (login, password, last_name, first_name, middle_name, is_blocked, role_id) VALUES
    (N'bibliotekar', N'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', N'Смирнова', N'Ольга', N'Викторовна',  0, 1),
    (N'admin',       N'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', N'Ковалёв',  N'Сергей', N'Петрович',   0, 2),
    (N'komplekt',    N'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', N'Морозова', N'Анна',   N'Игоревна',   0, 3);

INSERT INTO CopyStatus (name) VALUES
    (N'Доступен'), (N'Выдан'), (N'Списан'), (N'Утерян');

INSERT INTO LoanForm (name) VALUES
    (N'На дом'), (N'В читальный зал');

INSERT INTO Genre (name) VALUES
    (N'Русская классика'), (N'Зарубежная классика'), (N'Научная фантастика'),
    (N'Учебная литература'), (N'Детская литература');

INSERT INTO Publisher (name, city) VALUES
    (N'Эксмо',       N'Москва'),
    (N'АСТ',         N'Москва'),
    (N'Азбука',      N'Санкт-Петербург'),
    (N'Просвещение', N'Москва');

INSERT INTO Author (last_name, first_name, middle_name, country) VALUES
    (N'Гоголь',      N'Николай',   N'Васильевич',     N'Россия'),
    (N'Пушкин',      N'Александр', N'Сергеевич',      N'Россия'),
    (N'Толстой',     N'Лев',       N'Николаевич',     N'Россия'),
    (N'Достоевский', N'Фёдор',     N'Михайлович',     N'Россия'),
    (N'Тургенев',    N'Иван',      N'Сергеевич',      N'Россия'),
    (N'Чехов',       N'Антон',     N'Павлович',       N'Россия'),
    (N'Брэдбери',    N'Рэй',       NULL,              N'США'),
    (N'Шилдт',       N'Герберт',   NULL,              N'США');

INSERT INTO Book (title, [year], pages, publisher_id, genre_id) VALUES
    (N'Мёртвые души',                  1842, 352,  1, 1),
    (N'Евгений Онегин',                1833, 320,  3, 1),
    (N'Война и мир',                   1869, 1440, 1, 1),
    (N'Преступление и наказание',      1866, 672,  2, 1),
    (N'Отцы и дети',                   1862, 288,  1, 1),
    (N'Палата № 6',                    1892, 96,   3, 1),
    (N'451 градус по Фаренгейту',      1953, 256,  1, 2),
    (N'C# 8.0. Полное руководство',    2020, 816,  1, 4);

INSERT INTO BookAuthor (book_id, author_id) VALUES
    (1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6), (7, 7), (8, 8);

INSERT INTO Copy (inventory_number, price, arrival_date, location, book_id, status_id) VALUES
    (N'ИН-000101', 450.00, '2026-09-15', N'Абонемент',     1, 1),
    (N'ИН-000102', 450.00, '2026-09-15', N'Читальный зал', 1, 1),
    (N'ИН-000103', 450.00, '2026-09-15', N'Абонемент',     1, 2),
    (N'ИН-000104', 380.00, '2026-09-15', N'Абонемент',     2, 1),
    (N'ИН-000105', 380.00, '2026-09-15', N'Абонемент',     2, 1),
    (N'ИН-000106', 990.00, '2026-09-16', N'Читальный зал', 3, 1),
    (N'ИН-000107', 640.00, '2026-09-16', N'Абонемент',     4, 1),
    (N'ИН-000108', 640.00, '2026-09-16', N'Абонемент',     4, 1),
    (N'ИН-000109', 310.00, '2026-09-17', N'Абонемент',     5, 1),
    (N'ИН-000110', 190.00, '2026-09-17', N'Читальный зал', 6, 2),
    (N'ИН-000111', 520.00, '2026-10-01', N'Абонемент',     7, 1),
    (N'ИН-000112', 980.00, '2026-10-01', N'Читальный зал', 8, 1),
    (N'ИН-000113', 990.00, '2026-09-16', N'Абонемент',     3, 3);

INSERT INTO Reader (last_name, first_name, middle_name, birth_date, category, ticket_number, contact, status) VALUES
    (N'Иванов',     N'Андрей',  N'Александрович', '2004-03-12', N'студент',       N'БН-2023001', N'ivanov@student.vlsu.ru',     N'Активен'),
    (N'Петрова',    N'Мария',   N'Сергеевна',     '1988-07-25', N'преподаватель', N'БН-2021004', N'petrova@vlsu.ru',            N'Активен'),
    (N'Сидоров',    N'Кирилл',  N'Константинович','2005-11-02', N'студент',       N'БН-2024015', N'sidorov@student.vlsu.ru',    N'Активен'),
    (N'Кузнецова',  N'Елена',   N'Викторовна',    '2003-05-19', N'студент',       N'БН-2022021', N'kuznetsova@student.vlsu.ru', N'Активен');

INSERT INTO Acquisition ([date], quantity, price, source, book_id) VALUES
    ('2026-09-15', 3, 450.00, N'покупка', 1),
    ('2026-09-15', 2, 380.00, N'покупка', 2),
    ('2026-10-01', 1, 520.00, N'дар',     7);

INSERT INTO WriteOff ([date], reason, copy_id) VALUES
    ('2026-10-20', N'износ', 13);

INSERT INTO Loan (issue_date, due_date, return_date, is_overdue, reader_id, copy_id, form_id, user_id) VALUES
    ('2026-10-12', '2026-10-26', NULL, NULL, 3, 3, 1, 1),
    (CAST(GETDATE() AS DATE), CAST(GETDATE() AS DATE), NULL, NULL, 1, 10, 2, 1);

INSERT INTO Debt (days_overdue, created_at, closed_at, loan_id)
SELECT DATEDIFF(DAY, L.due_date, GETDATE()), GETDATE(), NULL, L.id
FROM Loan AS L
WHERE L.id = 1 AND L.return_date IS NULL AND L.due_date < GETDATE();

PRINT N'Тестовые данные загружены.';
GO
