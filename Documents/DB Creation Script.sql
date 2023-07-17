-- Drop all foreign key constraints
while(exists(select 1
             from INFORMATION_SCHEMA.TABLE_CONSTRAINTS
             where CONSTRAINT_TYPE = 'FOREIGN KEY'))
    begin
        declare @sql nvarchar(2000)
        SELECT TOP 1 @sql = ('ALTER TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME
            + '] DROP CONSTRAINT [' + CONSTRAINT_NAME + ']')
        FROM information_schema.table_constraints
        WHERE CONSTRAINT_TYPE = 'FOREIGN KEY'
        exec (@sql)
        PRINT @sql
    end

-- Drop all tables
while(exists(select 1
             from INFORMATION_SCHEMA.TABLES
             where TABLE_NAME != '__MigrationHistory'
               AND TABLE_TYPE = 'BASE TABLE'))
    begin
        SELECT TOP 1 @sql = ('DROP TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME
            + ']')
        FROM INFORMATION_SCHEMA.TABLES
        WHERE TABLE_NAME != '__MigrationHistory'
          AND TABLE_TYPE = 'BASE TABLE'
        exec (@sql)
        PRINT @sql
    end

CREATE TABLE [User]
(
    [Id]         INT IDENTITY (1,1) NOT NULL,
    [FirstName]  NVARCHAR(50)       NOT NULL,
    [LastName]   NVARCHAR(50)       NOT NULL,
    [Email]      NVARCHAR(256)      NOT NULL,
    [Password]   NVARCHAR(255)      NOT NULL,                   -- This should store hashed and salted passwords, not plain text
    [UserTypeId] INT                NOT NULL,
    [CreatedAt]  DATETIME2          NOT NULL DEFAULT GETDATE(), -- Timestamp of when the record was created
    [UpdatedAt]  DATETIME2,                                     -- Timestamp of the last update to the record
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_User_Email] UNIQUE ([Email])                 -- Ensure that each email is unique
);

-- Create UserType table to store different types of users
CREATE TABLE [UserType]
(
    [Id]        INT IDENTITY (1,1) NOT NULL,
    [Type]      NVARCHAR(20)       NOT NULL,
    [CreatedAt] DATETIME2          NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2,
    CONSTRAINT [PK_UserType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create SessionLog table to store user session information
CREATE TABLE [SessionLog]
(
    [Id]              INT IDENTITY (1,1) NOT NULL,
    [UserId]          INT                NOT NULL,
    [LogonTimeStamp]  DATETIME2          NOT NULL,
    [LogoffTimeStamp] DATETIME2          NOT NULL,
    [OperatingSystem] NVARCHAR(50)       NOT NULL,
    [ClientSoftware]  NVARCHAR(50)       NOT NULL,
    [RemoteIpAddress] NVARCHAR(50)       NOT NULL,
    [CreatedAt]       DATETIME2          NOT NULL DEFAULT GETDATE(),
    [UpdatedAt]       DATETIME2,
    CONSTRAINT [PK_SessionLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create PaymentStrategyPlan table to store information about each user's payment strategy
CREATE TABLE [PaymentStrategyPlan]
(
    [Id]                   INT IDENTITY (1,1) NOT NULL,
    [UserId]               INT                NOT NULL,
    [StrategyTypeId]       INT                NOT NULL,
    [GlobalMonthlyPayment] DECIMAL(10, 3)     NOT NULL, -- The total amount the user is able to pay across all loans each month
    [CreatedAt]            DATETIME2          NOT NULL DEFAULT GETDATE(),
    [UpdatedAt]            DATETIME2,
    CONSTRAINT [PK_PaymentStrategyPlan] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create Loan table to store information about each loan
CREATE TABLE [Loan]
(
    [Id]              INT IDENTITY (1,1) NOT NULL,
    [LoanNickName]    NVARCHAR(50)       NOT NULL,
    [PaymentStrategy] INT                NOT NULL,
    [Principal]       DECIMAL(10, 3)     NOT NULL,
    [InterestRate]    DECIMAL(5, 5)      NOT NULL,
    [Fees]            DECIMAL(10, 3)     NOT NULL,
    [MonthlyPayment]  DECIMAL(10, 3)     NOT NULL,
    [RemainingTerm]   INT                NOT NULL,
    [Currency]        INT                NOT NULL,
    [CreatedAt]       DATETIME2          NOT NULL DEFAULT GETDATE(),
    [UpdatedAt]       DATETIME2,
    CONSTRAINT [PK_Loan] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_Loan_LoanNickName] UNIQUE ([LoanNickName]) -- Ensure that each loan nickname is unique
);

-- Create Currency table to store different types of currencies
CREATE TABLE [Currency]
(
    [Id]         INT IDENTITY (1,1) NOT NULL,
    [FormalName] NVARCHAR(50)       NOT NULL,
    [ShortName]  NVARCHAR(20)       NOT NULL,
    [Symbol]     NVARCHAR(10)       NOT NULL,
    [CreatedAt]  DATETIME2          NOT NULL DEFAULT GETDATE(),
    [UpdatedAt]  DATETIME2,
    CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create DebtSnowflake table to store information about extra payments on the debts
CREATE TABLE [DebtSnowflake]
(
    [Id]              INT IDENTITY (1,1) NOT NULL,
    [Date]            DATE               NOT NULL,
    [Amount]          DECIMAL(10, 3)     NOT NULL,
    [PaymentStrategy] INT                NOT NULL,
    [CreatedAt]       DATETIME2          NOT NULL DEFAULT GETDATE(),
    [UpdatedAt]       DATETIME2,
    CONSTRAINT [PK_DebtSnowflake] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create StrategyType table to store different types of payment strategies
CREATE TABLE [StrategyType]
(
    [Id]                INT IDENTITY (1,1) NOT NULL,
    [Type]              NVARCHAR(20)       NOT NULL,
    [HasCustomStrategy] BIT                NOT NULL, -- Indicates whether the strategy is custom or not
    [CreatedAt]         DATETIME2          NOT NULL DEFAULT GETDATE(),
    [UpdatedAt]         DATETIME2,
    CONSTRAINT [PK_StrategyType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create LoanCardinalOrder table to store the order in which loans should be paid off
CREATE TABLE [LoanCardinalOrder]
(
    [Id]              INT IDENTITY (1,1) NOT NULL,
    [PaymentStrategy] INT                NOT NULL,
    [LoanId]          INT                NOT NULL,
    [CardinalOrder]   INT                NOT NULL, -- The order in which the loan should be paid off
    [CreatedAt]       DATETIME2          NOT NULL DEFAULT GETDATE(),
    [UpdatedAt]       DATETIME2,
    CONSTRAINT [PK_LoanCardinalOrder] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Add foreign key constraints to ensure data integrity
ALTER TABLE [User]
    ADD CONSTRAINT [FK_User_UserTypeId_UserType_Id] FOREIGN KEY ([UserTypeId])
        REFERENCES [UserType] ([Id]);

ALTER TABLE [SessionLog]
    ADD CONSTRAINT [FK_SessionLog_UserId_User_Id] FOREIGN KEY ([UserId])
        REFERENCES [User] ([Id]);

ALTER TABLE [PaymentStrategyPlan]
    ADD CONSTRAINT [FK_PaymentStrategyPlan_UserId_User_Id] FOREIGN KEY ([UserId])
        REFERENCES [User] ([Id]);

ALTER TABLE [PaymentStrategyPlan]
    ADD CONSTRAINT [FK_PaymentStrategyPlan_StrategyTypeId_StrategyType_Id] FOREIGN KEY ([StrategyTypeId])
        REFERENCES [StrategyType] ([Id]);

ALTER TABLE [Loan]
    ADD CONSTRAINT [FK_Loan_PaymentStrategy_PaymentStrategyPlan_Id] FOREIGN KEY ([PaymentStrategy])
        REFERENCES [PaymentStrategyPlan] ([Id]);

ALTER TABLE [Loan]
    ADD CONSTRAINT [FK_Loan_Currency_Currency_Id] FOREIGN KEY ([Currency])
        REFERENCES [Currency] ([Id]);

ALTER TABLE [DebtSnowflake]
    ADD CONSTRAINT [FK_DebtSnowflake_PaymentStrategy_PaymentStrategyPlan_Id] FOREIGN KEY ([PaymentStrategy])
        REFERENCES [PaymentStrategyPlan] ([Id]);

ALTER TABLE [LoanCardinalOrder]
    ADD CONSTRAINT [FK_LoanCardinalOrder_PaymentStrategy_PaymentStrategyPlan_Id] FOREIGN KEY ([PaymentStrategy])
        REFERENCES [PaymentStrategyPlan] ([Id]);

ALTER TABLE [LoanCardinalOrder]
    ADD CONSTRAINT [FK_LoanCardinalOrder_LoanId_Loan_Id] FOREIGN KEY ([LoanId])
        REFERENCES [Loan] ([Id]);


-- Insert data into UserType
INSERT INTO UserType (Type, CreatedAt, UpdatedAt)
VALUES ('Type1', GETDATE(), NULL),
       ('Type2', GETDATE(), NULL),
       ('Type3', GETDATE(), NULL);

-- Insert data into User
INSERT INTO [User] (FirstName, LastName, Email, Password, UserTypeId, CreatedAt, UpdatedAt)
VALUES ('John', 'Doe', 'john.doe@example.com', 'hashedpassword1', 1, GETDATE(), NULL),
       ('Jane', 'Doe', 'jane.doe@example.com', 'hashedpassword2', 2, GETDATE(), NULL),
       ('Jim', 'Doe', 'jim.doe@example.com', 'hashedpassword3', 3, GETDATE(), NULL);

-- Insert data into StrategyType
INSERT INTO StrategyType (Type, HasCustomStrategy, CreatedAt, UpdatedAt)
VALUES ('Strategy1', 0, GETDATE(), NULL),
       ('Strategy2', 1, GETDATE(), NULL),
       ('Strategy3', 0, GETDATE(), NULL);

-- Insert data into PaymentStrategyPlan
INSERT INTO PaymentStrategyPlan (UserId, StrategyTypeId, GlobalMonthlyPayment, CreatedAt, UpdatedAt)
VALUES (1, 1, 1000.00, GETDATE(), NULL),
       (2, 2, 2000.00, GETDATE(), NULL),
       (3, 3, 3000.00, GETDATE(), NULL);

-- Insert data into Currency
INSERT INTO Currency (FormalName, ShortName, Symbol, CreatedAt, UpdatedAt)
VALUES ('United States Dollar', 'USD', '$', GETDATE(), NULL),
       ('Euro', 'EUR', '€', GETDATE(), NULL),
       ('British Pound', 'GBP', '£', GETDATE(), NULL);

-- Insert data into Loan
INSERT INTO Loan (LoanNickName, PaymentStrategy, Principal, InterestRate, Fees, MonthlyPayment, RemainingTerm, Currency,
                  CreatedAt, UpdatedAt)
VALUES ('Loan 1', 1, 10000.00, 0.05, 100.00, 200.00, 60, 1, GETDATE(), NULL),
       ('Loan 2', 2, 20000.00, 0.06, 200.00, 400.00, 48, 2, GETDATE(), NULL),
       ('Loan 3', 3, 30000.00, 0.07, 300.00, 600.00, 36, 3, GETDATE(), NULL),
       ('Loan 4', 1, 40000.00, 0.08, 400.00, 800.00, 24, 1, GETDATE(), NULL),
       ('Loan 5', 2, 50000.00, 0.09, 500.00, 1000.00, 12, 2, GETDATE(), NULL);
