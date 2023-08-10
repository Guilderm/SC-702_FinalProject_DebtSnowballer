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

-- Create the Currencies table
CREATE TABLE Currencies
(
    [Id]          INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    [Name]        NVARCHAR(50)       NOT NULL,
    [AlphaCode]   NVARCHAR(3)        NOT NULL UNIQUE,
    [NumericCode] INT                NOT NULL,
    [Symbol]      NVARCHAR(5)        NOT NULL,
    [Precision]   INT                NOT NULL
);

-- Insert the data into the Currencies table
INSERT INTO Currencies (Name, AlphaCode, NumericCode, Symbol, Precision)
VALUES ('United States dollar', 'USD', 840, '$', 2),
       ('Euro', 'EUR', 978, '€', 2),
       ('Japanese yen', 'JPY', 392, '¥', 0),
       ('Pound sterling', 'GBP', 826, '£', 2),
       ('Australian dollar', 'AUD', 36, '$', 2),
       ('Canadian dollar', 'CAD', 124, '$', 2),
       ('Swiss franc', 'CHF', 756, 'Fr', 2),
       ('Renminbi (Chinese yuan)', 'CNY', 156, '¥', 2),
       ('Indian rupee', 'INR', 356, '₹', 2),
       ('Costa Rican colón', 'CRC', 188, '₡', 2);

-- Currency will be defined using ISO 4217
CREATE TABLE [ExchangeRate]
(
    [Id]             INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    [BaseCurrency]   NVARCHAR(3)        NOT NULL DEFAULT 'USD' REFERENCES [Currencies] (AlphaCode),
    [QuoteCurrency]  NVARCHAR(3)        NOT NULL DEFAULT 'USD' REFERENCES [Currencies] (AlphaCode),
    [ConversionRate] DECIMAL(19, 9)     NOT NULL,
    [NextUpdateTime] DATETIME2          NOT NULL
);


-- Create DebtStrategyType table to store different types of debt strategies
CREATE TABLE [DebtPayDownMethod]
(
    [Id]   INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(50)       NOT NULL
);

-- Insert the three debt strategies into the DebtStrategyType table
INSERT INTO [DebtPayDownMethod] (Name)
VALUES ('Strict Debt Snowball'),
       ('Debt Avalanche');

-- Create UserType table to store different types of users
CREATE TABLE [UserRole]
(
    [Id]          INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    [Name]        NVARCHAR(20)       NOT NULL,
    [Description] NVARCHAR(50)       NOT NULL
);

-- Insert data into UserType
INSERT INTO [UserRole] (Name, Description)
VALUES ('EndUser', 'The user that the application is intended for'),
       ('HelpDesk', 'Those provided technical and customer support'),
       ('Administrator', 'The administrators of the application');

CREATE TABLE [UserProfile]
(
    [Id]                       INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    [Auth0UserId]              NVARCHAR(75)       NOT NULL UNIQUE,
    [GivenName]                NVARCHAR(50)       NULL,
    [FamilyName]               NVARCHAR(50)       NULL,
    [NickName]                 NVARCHAR(50)       NULL,
    [FullName]                 NVARCHAR(100)      NULL,
    [Email]                    NVARCHAR(256)      NULL,
    [Picture]                  NVARCHAR(MAX)      NULL,
    [Locale]                   NVARCHAR(10)       NULL,
    [UserRoleId]               INT                NOT NULL DEFAULT 1 REFERENCES [UserRole] (Id),
    [CreatedAt]                DATETIME2          NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [LastUpdated]              DATETIME2          NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- consider putting these into a different table called UserSettings
    [BaseCurrency]             NVARCHAR(3)        NOT NULL DEFAULT 'USD', -- Currency will be defined using ISO 4217
    --[DebtPlanMonthlyPayment] DECIMAL(18, 2)     NOT NULL DEFAULT 0 CHECK (DebtPlanMonthlyPayment >= AggregatedMonthlyPayment),
    [DebtPlanMonthlyPayment]   DECIMAL(18, 2)     NOT NULL DEFAULT 0 CHECK (DebtPlanMonthlyPayment >= 0),
    [SelectedStrategy]         INT                NOT NULL DEFAULT 1 REFERENCES [DebtPayDownMethod] (Id),

    -- Consider deleting these
    [TotalAmountOwed]          DECIMAL(18, 2)     NOT NULL DEFAULT 0,
    [AggregatedMonthlyPayment] DECIMAL(18, 2)     NOT NULL DEFAULT 0 CHECK (AggregatedMonthlyPayment >= 0),
);


-- Create SessionLog table to store user session information
CREATE TABLE [SessionLog]
(
    [Id]              INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    [Auth0UserId]     NVARCHAR(75)       NOT NULL FOREIGN KEY REFERENCES [UserProfile] (Auth0UserId),
    [LogonTimeStamp]  DATETIME2          NOT NULL DEFAULT GETDATE(),
    [LogoffTimeStamp] DATETIME2,
    [OperatingSystem] NVARCHAR(50)       NOT NULL,
    [ClientSoftware]  NVARCHAR(50)       NOT NULL,
    [RemoteIpAddress] NVARCHAR(50)       NOT NULL
);

-- Create Loan table to store information about each loan
CREATE TABLE [LoanDetail]
(
    [Id]                       INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    [Auth0UserId]              NVARCHAR(75)      NOT NULL FOREIGN KEY REFERENCES [UserProfile] (Auth0UserId),
    [Name]                     NVARCHAR(50)       NOT NULL,
    [RemainingPrincipal]       DECIMAL(18, 2)     NOT NULL CHECK (RemainingPrincipal >= 0),
    [BankFees]                 DECIMAL(18, 2)     NOT NULL CHECK (BankFees >= 0),
    [ContractedMonthlyPayment] DECIMAL(18, 2)     NOT NULL CHECK (ContractedMonthlyPayment >= 0),
    [AnnualInterestRate]       DECIMAL(6, 4)      NOT NULL CHECK (AnnualInterestRate >= 0),
    [MonthlyInterestRate]      DECIMAL(6, 4),    CHECK (MonthlyInterestRate >= 0),
    [RemainingTermInMonths]    INT                NOT NULL CHECK (RemainingTermInMonths >= 0),
    [CurrencyCode]             NVARCHAR(3)        NOT NULL DEFAULT 'USD' REFERENCES [Currencies] (AlphaCode),
    [CardinalOrder]            INT                NOT NULL,
    [StartDate]                DATE               NOT NULL DEFAULT (DATEFROMPARTS(YEAR(CURRENT_TIMESTAMP),
                                                                                  MONTH(CURRENT_TIMESTAMP), 1))
);

CREATE TABLE [PlannedSnowflakes]
(
    [Id]                INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    [Auth0UserId]       NVARCHAR(75)       NOT NULL FOREIGN KEY REFERENCES [UserProfile] (Auth0UserId),
    [Name]              NVARCHAR(50)       NOT NULL,
    [FrequencyInMonths] INT                NOT NULL CHECK (FrequencyInMonths >= 0),
    [Amount]            DECIMAL(18, 2)     NOT NULL CHECK (Amount >= 0),
    [StartingAt]        DATE               NOT NULL DEFAULT (DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)),
    EndingAt            DATE               NOT NULL DEFAULT DATEADD(YEAR, 45, CURRENT_TIMESTAMP),
    [CurrencyCode]      NVARCHAR(3)        NOT NULL DEFAULT 'USD' REFERENCES [Currencies] (AlphaCode),
);

-- Insert data into UserProfile
INSERT INTO UserProfile (auth0UserId, GivenName, FamilyName, Email, UserRoleId)
VALUES ('google-oauth2|116471976465148595031', 'Jim', 'Doe', 'jim.doe@example.com', 1),
       ('auth0|64cc577de4780fda44d0d662', 'Jim', 'Doe', 'jim.doe@example.com', 1);


-- Insert data into Loan
INSERT INTO LoanDetail (Auth0UserId, Name, RemainingPrincipal, AnnualInterestRate, BankFees, ContractedMonthlyPayment,
                        RemainingTermInMonths, CurrencyCode, CardinalOrder)
VALUES ('auth0|64cc577de4780fda44d0d662', 'Home Mortgage', 125000000, 0.125, 0, 562500, 360, 'CRC', 1),
       ('auth0|64cc577de4780fda44d0d662', '2023 Honda Accord Loan', 25000, 0.042, 0, 460, 60, 'USD', 2),
       ('auth0|64cc577de4780fda44d0d662', 'Visa Credit Card', 3125000, 0.229, 0, 625000, 60, 'CRC', 3),
       ('auth0|64cc577de4780fda44d0d662', 'Federal Student Loan', 30000, 0.058, 0, 350, 120, 'USD', 4),
       ('auth0|64cc577de4780fda44d0d662', 'Personal Loan', 10000, 0.1, 0, 200, 60, 'USD', 5);


INSERT INTO LoanDetail (Auth0UserId, Name, RemainingPrincipal, AnnualInterestRate, BankFees, ContractedMonthlyPayment,
                        RemainingTermInMonths, CurrencyCode, CardinalOrder)
VALUES ('google-oauth2|116471976465148595031', 'Home Mortgage', 250000, 0.035, 0, 1122.61, 360, 'USD', 1),
       ('google-oauth2|116471976465148595031', '2024 Toyota Camry Loan', 22000, 0.045, 200, 411.53, 60, 'USD', 2),
       ('google-oauth2|116471976465148595031', 'Personal Loan', 10000, 0.07, 0, 232.22, 48, 'USD', 3),
       ('google-oauth2|116471976465148595031', 'Visa Credit Card', 5000, 0.18, 50, 180, 36, 'USD', 4),
       ('google-oauth2|116471976465148595031', 'Home Renovation Loan', 5000000, 0.1, 0, 161334.64, 36, 'CRC', 5),
       ('google-oauth2|116471976465148595031', 'Education Loan', 2000000, 0.08, 10000, 62650.96, 36, 'CRC', 6);

-- Insert data into Snowflakes
INSERT INTO [PlannedSnowflakes] (Auth0UserId, Name, FrequencyInMonths, Amount, StartingAt, EndingAt, CurrencyCode)
VALUES ('google-oauth2|116471976465148595031', 'Snowflake1', 12, 1000.00, GETDATE(), DATEADD(YEAR, 45, GETDATE()),
        'USD'),
       ('google-oauth2|116471976465148595031', 'Snowflake2', 6, 500.00, GETDATE(), DATEADD(YEAR, 45, GETDATE()), 'CRC'),
       ('google-oauth2|116471976465148595031', 'Snowflake3', 3, 200.00, GETDATE(), DATEADD(YEAR, 45, GETDATE()), 'USD'),
       ('google-oauth2|116471976465148595031', 'Snowflake4', 12, 1500.00, GETDATE(), DATEADD(YEAR, 45, GETDATE()),
        'CRC'),
       ('google-oauth2|116471976465148595031', 'Snowflake5', 5, 750.00, GETDATE(), DATEADD(YEAR, 45, GETDATE()), 'USD'),

       ('auth0|64cc577de4780fda44d0d662', 'Snowflake1', 12, 1000.00, GETDATE(), DATEADD(YEAR, 45, GETDATE()),
        'USD'),
       ('auth0|64cc577de4780fda44d0d662', 'Snowflake2', 6, 500.00, GETDATE(), DATEADD(YEAR, 45, GETDATE()), 'CRC'),
       ('auth0|64cc577de4780fda44d0d662', 'Snowflake3', 3, 200.00, GETDATE(), DATEADD(YEAR, 45, GETDATE()), 'USD'),
       ('auth0|64cc577de4780fda44d0d662', 'Snowflake4', 12, 1500.00, GETDATE(), DATEADD(YEAR, 45, GETDATE()),
        'CRC'),
       ('auth0|64cc577de4780fda44d0d662', 'Snowflake5', 5, 750.00, GETDATE(), DATEADD(YEAR, 45, GETDATE()), 'USD');