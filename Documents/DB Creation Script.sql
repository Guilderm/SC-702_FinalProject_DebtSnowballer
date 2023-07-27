-- Drop all foreign key constraints
while(exists(select 1
             from INFORMATION_SCHEMA.TABLE_CONSTRAINTS
             where CONSTRAINT_TYPE = 'FOREIGN KEY')) begin
    declare @sql nvarchar(2000)
    SELECT TOP 1
            @sql = ('ALTER TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + '] DROP CONSTRAINT [' + CONSTRAINT_NAME + ']')
    FROM information_schema.table_constraints
    WHERE CONSTRAINT_TYPE = 'FOREIGN KEY'
    exec (@sql)
    PRINT @sql
end

-- Drop all tables
while(exists(select 1
             from INFORMATION_SCHEMA.TABLES
             where TABLE_NAME != '__MigrationHistory'
               AND TABLE_TYPE = 'BASE TABLE')) begin
    SELECT TOP 1 @sql = ('DROP TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + ']')
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME != '__MigrationHistory'
      AND TABLE_TYPE = 'BASE TABLE'
    exec (@sql)
    PRINT @sql
end


CREATE TABLE [UserProfile]
(
    [Id]           INT IDENTITY (1,1) NOT NULL,
    [Auth0UserId]  NVARCHAR(125)      NOT NULL UNIQUE,        -- Make this column non-nullable
    [FirstName]    NVARCHAR(50)       NULL,
    [LastName]     NVARCHAR(50)       NULL,
    [Email]        NVARCHAR(256)      NULL,
    [BaseCurrency] NVARCHAR(3)         NOT NULL     DEFAULT 'USD', -- Currency will be defined using ISO 4217
    [UserTypeId]   INT                NOT NULL DEFAULT 1,
    [CreatedAt]    DATETIME2          NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create UserType table to store different types of users
CREATE TABLE [UserType]
(
    [Id]          INT IDENTITY (1,1) NOT NULL,
    [Type]        NVARCHAR(20)       NOT NULL,
    [Description] NVARCHAR(50)       NOT NULL,
    CONSTRAINT [PK_UserType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Insert data into UserType
INSERT INTO UserType (Type, Description)
VALUES ('EndUser', 'The user that the application is inteded for'),
       ('HelpDesk', 'Those provideind technical and customer support'),
       ('Administrator', 'The administrotors of the application');

-- Create SessionLog table to store user session information
CREATE TABLE [SessionLog]
(
    [Id]              INT IDENTITY (1,1) NOT NULL,
    [UserId]          INT                NOT NULL FOREIGN KEY REFERENCES [UserProfile] (ID),
    [LogonTimeStamp]  DATETIME2          NOT NULL DEFAULT GETDATE(),
    [LogoffTimeStamp] DATETIME2,
    [OperatingSystem] NVARCHAR(50)       NOT NULL,
    [ClientSoftware]  NVARCHAR(50)       NOT NULL,
    [RemoteIpAddress] NVARCHAR(50)       NOT NULL,
    CONSTRAINT [PK_SessionLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create DebtStrategyType table to store different types of debt strategies
CREATE TABLE [StrategyType]
(
    [Id]   INT IDENTITY (1,1) NOT NULL,
    [Type] NVARCHAR(50)       NOT NULL,
    CONSTRAINT [PK_DebtStrategyType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Insert the three debt strategies into the DebtStrategyType table
INSERT INTO StrategyType (Type)
VALUES ('Debt Snowball'),
       ('Strict Debt Snowball'),
       ('Debt Avalanche');

-- Create DebtStrategy table to store the chosen strategy for each user
CREATE TABLE [DebtStrategy]
(
    [Id]          INT IDENTITY (1,1) NOT NULL,
    [Auth0UserId] NVARCHAR(125)      NOT NULL,
    [UserId]      INT                NOT NULL FOREIGN KEY REFERENCES [UserProfile] (ID),
    [StrategyId]  INT                NOT NULL FOREIGN KEY REFERENCES [StrategyType] (ID),
    CONSTRAINT [PK_DebtStrategy] PRIMARY KEY CLUSTERED ([Id] ASC)
);


-- Create Loan table to store information about each loan
CREATE TABLE [Debt]
(
    [Id]             INT IDENTITY (1,1) NOT NULL,
    -- TODO: reconfigure the foren key:
    -- [Auth0UserId]    NVARCHAR(125)      NOT NULL FOREIGN KEY REFERENCES [UserProfile] (Auth0UserId),
    [Auth0UserId]    NVARCHAR(125)      NOT NULL,
    [LoanNickName]   NVARCHAR(50)       NOT NULL,
    [Principal]      DECIMAL(10, 3)     NOT NULL,
    [InterestRate]   DECIMAL(5, 5)      NOT NULL,
    [Fees]           DECIMAL(10, 3)     NOT NULL,
    [MonthlyPayment] DECIMAL(10, 3)     NOT NULL,
    [RemainingTerm]  INT                NOT NULL,
    [CurrencyCode]  NVARCHAR(3)         NOT NULL     DEFAULT 'USD', -- Currency will be defined using ISO 4217
    [CardinalOrder]  INT                NOT NULL, -- The order in which the loan should be paid off
    [CreatedAt]      DATETIME2          NOT NULL                                        DEFAULT GETDATE(),
    CONSTRAINT [PK_Loan] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create PaymentStrategyPlan table to store information about each user's payment strategy
CREATE TABLE [MonthlyExtraPayments]
(
    [Id]     INT IDENTITY (1,1) NOT NULL,
    [UserId] INT                NOT NULL FOREIGN KEY REFERENCES [UserProfile] (ID),
    [Amount] DECIMAL(10, 3)     NOT NULL,
    CONSTRAINT [PK_MonthlyExtraPayments] PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [OnetimeExtraPayments]
(
    [Id]     INT IDENTITY (1,1) NOT NULL,
    [UserId] INT                NOT NULL FOREIGN KEY REFERENCES [UserProfile] (ID),
    [Amount] DECIMAL(10, 3)     NOT NULL,
    [Date]   DATETIME2          NOT NULL,
    CONSTRAINT [PK_OnetimeExtraPayments] PRIMARY KEY CLUSTERED ([Id] ASC)
);


-- Insert data into UserProfile
INSERT INTO UserProfile (auth0UserId, FirstName, LastName, Email, UserTypeId)
VALUES ('auth0|60d7b7f29b14170068e3244f', 'John', 'Doe', 'john.doe@example.com', 1),
       ('auth0|60d7b7f29b14170068e32450', 'Jane', 'Doe', 'jane.doe@example.com', 2),
       ('auth0|60d7b7f29b14170068e32451', 'Jim', 'Doe', 'jim.doe@example.com', 3);

-- Insert data into Loan
INSERT INTO Debt (Auth0UserId, LoanNickName, Principal, InterestRate, Fees, MonthlyPayment, RemainingTerm, CurrencyCode,
                  CardinalOrder)
VALUES ('auth0|60d7b7f29b14170068e3244f', 'Loan 1', 10000.00, 0.05, 100.00, 200.00, 60, 'CRC', 1),
       ('auth0|60d7b7f29b14170068e32450', 'Loan 2', 20000.00, 0.06, 200.00, 400.00, 48, 'CRC', 2),
       ('auth0|60d7b7f29b14170068e32451', 'Loan 3', 30000.00, 0.07, 300.00, 600.00, 36, 'CRC', 3),

       ('google-oauth2|116471976465148595031', 'Personal', 5000.00, 0.05, 100.00, 200.00, 60, 'CRC', 1),
       ('google-oauth2|116471976465148595031', 'Car', 15000.00, 0.06, 200.00, 400.00, 48, 'CRC', 2),
       ('google-oauth2|116471976465148595031', 'House', 50000.00, 0.07, 300.00, 600.00, 36, 'CRC', 3),

       ('05d1O2h3eSwYig2VqhcEgw3IpT0FGIZs', 'Personal', 5000.00, 0.05, 100.00, 200.00, 60, 'CRC', 1),
       ('05d1O2h3eSwYig2VqhcEgw3IpT0FGIZs', 'Car', 15000.00, 0.06, 200.00, 400.00, 48, 'CRC', 2),
       ('05d1O2h3eSwYig2VqhcEgw3IpT0FGIZs', 'House', 50000.00, 0.07, 300.00, 600.00, 36, 'CRC', 3);

-- Insert data into MonthlyExtraPayments
INSERT INTO MonthlyExtraPayments (UserId, Amount)
VALUES (1, 100.00),
       (2, 200.00),
       (3, 300.00);

-- Insert data into OnetimeExtraPayments
INSERT INTO OnetimeExtraPayments (UserId, Amount, Date)
VALUES (1, 1000.00, '2023-07-18'),
       (2, 2000.00, '2023-07-18'),
       (3, 3000.00, '2023-07-18');



-------------
--TO DELETE--
-------------


-- Create CRUD table with different column types for testing
CREATE TABLE [CRUD]
(
    [Id]           INT IDENTITY (1,1) NOT NULL,
    [LoanName]     NVARCHAR(50)       NOT NULL,
    [Principal]    DECIMAL(18, 2)     NOT NULL,
    [InterestRate] DECIMAL(5, 2)      NOT NULL,
    [TermMonths]   INT                NOT NULL,
    [StartDate]    DATETIME2          NOT NULL,
    [EndDate]      DATETIME2,
    CONSTRAINT [PK_CRUD] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Insert sample data into CRUD
INSERT INTO [CRUD] (LoanName, Principal, InterestRate, TermMonths, StartDate)
VALUES ('Loan 1', 10000.00, 5.00, 60, GETDATE()),
       ('Loan 2', 20000.00, 4.50, 48, GETDATE()),
       ('Loan 3', 30000.00, 4.00, 36, GETDATE()),
       ('Loan 4', 40000.00, 3.50, 24, GETDATE()),
       ('Loan 5', 50000.00, 3.00, 12, GETDATE()),
       ('Loan 6', 60000.00, 2.50, 60, GETDATE()),
       ('Loan 7', 70000.00, 2.00, 48, GETDATE()),

       ('Loan 8', 80000.00, 1.50, 36, GETDATE()),
       ('Loan 9', 90000.00, 1.00, 24, GETDATE()),
       ('Loan 10', 100000.00, 0.50, 12, GETDATE());