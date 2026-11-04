USE master;
GO

IF DB_ID(N'LibraryDb') IS NULL
    CREATE DATABASE LibraryDb;
GO

USE LibraryDb;
GO

CREATE TABLE Role (
    id          INT IDENTITY (1,1) NOT NULL,
    name        VARCHAR(50)  NOT NULL,
    description VARCHAR(255) NULL,
    CONSTRAINT PK_Role PRIMARY KEY (id),
    CONSTRAINT UQ_Role_name UNIQUE (name)
);
GO

CREATE TABLE [User] (
    id          INT IDENTITY (1,1) NOT NULL,
    login       VARCHAR(50)  NOT NULL,
    password    VARCHAR(50)  NOT NULL,
    last_name   VARCHAR(100) NOT NULL,
    first_name  VARCHAR(100) NOT NULL,
    middle_name VARCHAR(100) NULL,
    is_blocked  BIT          NOT NULL CONSTRAINT DF_User_is_blocked DEFAULT (0),
    role_id     INT          NOT NULL,
    CONSTRAINT PK_User PRIMARY KEY (id),
    CONSTRAINT UQ_User_login UNIQUE (login),
    CONSTRAINT FK_User_Role FOREIGN KEY (role_id) REFERENCES Role (id)
);
GO

CREATE TABLE Genre (
    id   INT IDENTITY (1,1) NOT NULL,
    name VARCHAR(100) NOT NULL,
    CONSTRAINT PK_Genre PRIMARY KEY (id),
    CONSTRAINT UQ_Genre_name UNIQUE (name)
);
GO

CREATE TABLE Publisher (
    id   INT IDENTITY (1,1) NOT NULL,
    name VARCHAR(150) NOT NULL,
    city VARCHAR(100) NULL,
    CONSTRAINT PK_Publisher PRIMARY KEY (id),
    CONSTRAINT UQ_Publisher_name UNIQUE (name)
);
GO

CREATE TABLE Author (
    id          INT IDENTITY (1,1) NOT NULL,
    last_name   VARCHAR(100) NOT NULL,
    first_name  VARCHAR(100) NOT NULL,
    middle_name VARCHAR(100) NULL,
    country     VARCHAR(100) NULL,
    CONSTRAINT PK_Author PRIMARY KEY (id)
);
GO

CREATE TABLE Book (
    id           INT IDENTITY (1,1) NOT NULL,
    title        VARCHAR(255) NOT NULL,
    [year]       INT          NOT NULL,
    pages        INT          NULL,
    publisher_id INT          NOT NULL,
    genre_id     INT          NOT NULL,
    CONSTRAINT PK_Book PRIMARY KEY (id),
    CONSTRAINT FK_Book_Publisher FOREIGN KEY (publisher_id) REFERENCES Publisher (id),
    CONSTRAINT FK_Book_Genre     FOREIGN KEY (genre_id)     REFERENCES Genre (id)
);
GO

CREATE TABLE BookAuthor (
    book_id   INT NOT NULL,
    author_id INT NOT NULL,
    CONSTRAINT PK_BookAuthor PRIMARY KEY (book_id, author_id),
    CONSTRAINT FK_BookAuthor_Book   FOREIGN KEY (book_id)   REFERENCES Book (id),
    CONSTRAINT FK_BookAuthor_Author FOREIGN KEY (author_id) REFERENCES Author (id)
);
GO

CREATE TABLE CopyStatus (
    id   INT IDENTITY (1,1) NOT NULL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT PK_CopyStatus PRIMARY KEY (id),
    CONSTRAINT UQ_CopyStatus_name UNIQUE (name)
);
GO

CREATE TABLE Copy (
    id               INT IDENTITY (1,1) NOT NULL,
    inventory_number VARCHAR(20)     NOT NULL,
    price            DECIMAL(10, 2)  NULL,
    arrival_date     DATE            NOT NULL,
    location         VARCHAR(50)     NOT NULL,
    book_id          INT             NOT NULL,
    status_id        INT             NOT NULL,
    CONSTRAINT PK_Copy PRIMARY KEY (id),
    CONSTRAINT UQ_Copy_inventory_number UNIQUE (inventory_number),
    CONSTRAINT FK_Copy_Book        FOREIGN KEY (book_id)   REFERENCES Book (id),
    CONSTRAINT FK_Copy_CopyStatus  FOREIGN KEY (status_id) REFERENCES CopyStatus (id)
);
GO

CREATE TABLE Reader (
    id            INT IDENTITY (1,1) NOT NULL,
    last_name     VARCHAR(100) NOT NULL,
    first_name    VARCHAR(100) NOT NULL,
    middle_name   VARCHAR(100) NULL,
    birth_date    DATE         NULL,
    category      VARCHAR(50)  NOT NULL,
    ticket_number VARCHAR(20)  NOT NULL,
    contact       VARCHAR(150) NULL,
    status        VARCHAR(20)  NOT NULL,
    CONSTRAINT PK_Reader PRIMARY KEY (id),
    CONSTRAINT UQ_Reader_ticket_number UNIQUE (ticket_number)
);
GO

CREATE TABLE LoanForm (
    id   INT IDENTITY (1,1) NOT NULL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT PK_LoanForm PRIMARY KEY (id),
    CONSTRAINT UQ_LoanForm_name UNIQUE (name)
);
GO

CREATE TABLE Acquisition (
    id       INT IDENTITY (1,1) NOT NULL,
    [date]   DATE           NOT NULL,
    quantity INT            NOT NULL,
    price    DECIMAL(10, 2) NULL,
    source   VARCHAR(50)    NOT NULL,
    book_id  INT            NOT NULL,
    CONSTRAINT PK_Acquisition PRIMARY KEY (id),
    CONSTRAINT FK_Acquisition_Book FOREIGN KEY (book_id) REFERENCES Book (id)
);
GO

CREATE TABLE WriteOff (
    id      INT IDENTITY (1,1) NOT NULL,
    [date]  DATE          NOT NULL,
    reason  VARCHAR(100)  NOT NULL,
    copy_id INT           NOT NULL,
    CONSTRAINT PK_WriteOff PRIMARY KEY (id),
    CONSTRAINT FK_WriteOff_Copy FOREIGN KEY (copy_id) REFERENCES Copy (id)
);
GO

CREATE TABLE Loan (
    id          INT IDENTITY (1,1) NOT NULL,
    issue_date  DATE  NOT NULL,
    due_date    DATE  NOT NULL,
    return_date DATE  NULL,
    is_overdue  BIT   NULL,
    reader_id   INT   NOT NULL,
    copy_id     INT   NOT NULL,
    form_id     INT   NOT NULL,
    user_id     INT   NOT NULL,
    CONSTRAINT PK_Loan PRIMARY KEY (id),
    CONSTRAINT FK_Loan_Reader   FOREIGN KEY (reader_id) REFERENCES Reader (id),
    CONSTRAINT FK_Loan_Copy     FOREIGN KEY (copy_id)   REFERENCES Copy (id),
    CONSTRAINT FK_Loan_LoanForm FOREIGN KEY (form_id)   REFERENCES LoanForm (id),
    CONSTRAINT FK_Loan_User     FOREIGN KEY (user_id)   REFERENCES [User] (id)
);
GO

CREATE TABLE LoanExtension (
    id             INT IDENTITY (1,1) NOT NULL,
    old_due_date   DATE NOT NULL,
    new_due_date   DATE NOT NULL,
    extension_date DATE NOT NULL,
    loan_id        INT  NOT NULL,
    CONSTRAINT PK_LoanExtension PRIMARY KEY (id),
    CONSTRAINT FK_LoanExtension_Loan FOREIGN KEY (loan_id) REFERENCES Loan (id)
);
GO

CREATE TABLE Debt (
    id           INT IDENTITY (1,1) NOT NULL,
    days_overdue INT  NOT NULL,
    created_at   DATE NOT NULL,
    closed_at    DATE NULL,
    loan_id      INT  NOT NULL,
    CONSTRAINT PK_Debt PRIMARY KEY (id),
    CONSTRAINT FK_Debt_Loan FOREIGN KEY (loan_id) REFERENCES Loan (id)
);
GO

CREATE TABLE OperationLog (
    id          INT IDENTITY (1,1) NOT NULL,
    operation   VARCHAR(100)    NOT NULL,
    description VARCHAR(500)    NULL,
    created_at  DATETIME        NOT NULL CONSTRAINT DF_OperationLog_created_at DEFAULT (GETDATE()),
    user_id     INT             NOT NULL,
    CONSTRAINT PK_OperationLog PRIMARY KEY (id),
    CONSTRAINT FK_OperationLog_User FOREIGN KEY (user_id) REFERENCES [User] (id)
);
GO

PRINT N'База данных LibraryDb успешно создана: 17 таблиц.';
GO
