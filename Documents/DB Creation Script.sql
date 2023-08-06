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
    Id          INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    Name        NVARCHAR(50)       NOT NULL,
    AlphaCode   NVARCHAR(3)        NOT NULL UNIQUE,
    NumericCode INT                NOT NULL,
    Symbol      NVARCHAR(5)        NOT NULL,
    Precision   INT                NOT NULL
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
CREATE TABLE ExchangeRates
(
    Id             INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    BaseCurrency   NVARCHAR(3)        NOT NULL DEFAULT 'USD' REFERENCES [Currencies] (AlphaCode),
    QuoteCurrency  NVARCHAR(3)        NOT NULL DEFAULT 'USD' REFERENCES [Currencies] (AlphaCode),
    ConversionRate DECIMAL(19, 9)     NOT NULL,
    NextUpdateTime DATETIME2          NOT NULL
);


-- Create DebtStrategyType table to store different types of debt strategies
CREATE TABLE [StrategyType]
(
    [Id]   INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(50)       NOT NULL
);

-- Insert the three debt strategies into the DebtStrategyType table
INSERT INTO StrategyType (Name)
VALUES        ('Strict Debt Snowball'),
       ('Debt Avalanche');

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
    [UserTypeId]               INT                NOT NULL DEFAULT 1,
    [CreatedAt]                DATETIME2          NOT NULL DEFAULT GETDATE(),
    [LastUpdated]              DATETIME2          NOT NULL DEFAULT GETDATE(),

    -- consider putting these into a different table called UserSettings
    [BaseCurrency]             NVARCHAR(3)        NOT NULL DEFAULT 'USD', -- Currency will be defined using ISO 4217
    [DebtPlanMonthlyPayment]  DECIMAL(18, 2)     NOT NULL     DEFAULT 0,     -- Cannot be less than AggregatedMonthlyPayment
    [SelectedStrategy]         INT                NOT NULL     DEFAULT 1 REFERENCES [StrategyType] (Id),

    -- Consider deleting these
    [TotalAmountOwed]          DECIMAL(18, 2)     NOT NULL     DEFAULT 0,
    [AggregatedMonthlyPayment] DECIMAL(18, 2)     NOT NULL     DEFAULT 0,
);

-- Create UserType table to store different types of users
CREATE TABLE [UserType]
(
    [Id]          INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    [Type]        NVARCHAR(20)       NOT NULL,
    [Description] NVARCHAR(50)       NOT NULL
);

-- Insert data into UserType
INSERT INTO UserType (Type, Description)
VALUES ('EndUser', 'The user that the application is intended for'),
       ('HelpDesk', 'Those provided technical and customer support'),
       ('Administrator', 'The administrators of the application');

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
CREATE TABLE [Debt]
(
    [Id]                    INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    [Auth0UserId]           NVARCHAR(75)       NOT NULL FOREIGN KEY REFERENCES [UserProfile] (Auth0UserId),
    [NickName]              NVARCHAR(50)       NOT NULL,
    [RemainingPrincipal]    DECIMAL(18, 2)     NOT NULL,
    [BankFees]              DECIMAL(18, 2)     NOT NULL,
    [MonthlyPayment]        DECIMAL(18, 2)     NOT NULL,
    [InterestRate]          DECIMAL(6, 4)      NOT NULL,
    [RemainingTermInMonths] INT                NOT NULL,
    [CurrencyCode]          NVARCHAR(3)        NOT NULL DEFAULT 'USD' REFERENCES [Currencies] (AlphaCode),
    [CardinalOrder]         INT                NOT NULL,
    [CreatedAt]             DATETIME2          NOT NULL DEFAULT GETDATE()
);


CREATE TABLE Snowflakes
(
    Id                INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
    [Auth0UserId]     NVARCHAR(75)       NOT NULL FOREIGN KEY REFERENCES [UserProfile] (Auth0UserId),
    [NickName]        NVARCHAR(50)       NOT NULL,
    FrequencyInMonths INT                NOT NULL,
    Amount            DECIMAL(18, 2)     NOT NULL,
    [StartingAt]      DATETIME2          NOT NULL DEFAULT GETDATE(),
    [EndingAt]        DATETIME2          NOT NULL DEFAULT DATEADD(YEAR, 45, GETDATE()),
    [CurrencyCode]    NVARCHAR(3)        NOT NULL DEFAULT 'USD' REFERENCES [Currencies] (AlphaCode),
);

-- Insert data into UserProfile
INSERT INTO UserProfile (auth0UserId, GivenName, FamilyName, Email, UserTypeId)
VALUES ('google-oauth2|116471976465148595031', 'Jim', 'Doe', 'jim.doe@example.com', 1),
       ('auth0|64cc577de4780fda44d0d662', 'Jim', 'Doe', 'jim.doe@example.com', 1);

-- Insert data into Loan
INSERT INTO Debt (Auth0UserId, NickName, RemainingPrincipal, InterestRate, BankFees, MonthlyPayment,
                  RemainingTermInMonths, CurrencyCode, CardinalOrder)
VALUES ('google-oauth2|116471976465148595031', 'Home Mortgage', 125000000, 0.125, 0, 562500, 360, 'CRC', 1),
       ('google-oauth2|116471976465148595031', '2023 Honda Accord Loan', 25000, 0.042, 0, 460, 60, 'USD', 2),
       ('google-oauth2|116471976465148595031', 'Visa Credit Card', 3125000, 0.229, 0, 625000, 60, 'CRC', 3),
       ('google-oauth2|116471976465148595031', 'Federal Student Loan', 30000, 0.058, 0, 350, 120, 'USD', 4),
       ('google-oauth2|116471976465148595031', 'Personal Loan', 10000, 0.1, 0, 200, 60, 'USD', 5),

       ('auth0|64cc577de4780fda44d0d662', 'Home Mortgage', 125000000, 0.125, 0, 562500, 360, 'CRC', 1),
       ('auth0|64cc577de4780fda44d0d662', '2023 Honda Accord Loan', 25000, 0.042, 0, 460, 60, 'USD', 2),
       ('auth0|64cc577de4780fda44d0d662', 'Visa Credit Card', 3125000, 0.229, 0, 625000, 60, 'CRC', 3),
       ('auth0|64cc577de4780fda44d0d662', 'Federal Student Loan', 30000, 0.058, 0, 350, 120, 'USD', 4),
       ('auth0|64cc577de4780fda44d0d662', 'Personal Loan', 10000, 0.1, 0, 200, 60, 'USD', 5);


-- Insert data into Snowflakes
INSERT INTO Snowflakes (Auth0UserId, NickName, FrequencyInMonths, Amount, StartingAt, EndingAt, CurrencyCode)
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