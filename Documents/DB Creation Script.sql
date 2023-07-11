-- Create User table to store user information
CREATE TABLE [User] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [Email] NVARCHAR(MAX) NOT NULL,
    [Password] NVARCHAR(255) NOT NULL, -- This should store hashed and salted passwords, not plain text
    [UserTypeId] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- Timestamp of when the record was created
    [UpdatedAt] DATETIME2, -- Timestamp of the last update to the record
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_User_Email] UNIQUE ([Email]) -- Ensure that each email is unique
);

-- Create UserType table to store different types of users
CREATE TABLE [UserType] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Type] NVARCHAR(20) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2,
    CONSTRAINT [PK_UserType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create SessionLog table to store user session information
CREATE TABLE [SessionLog] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [LogonTimeStamp] DATETIME2 NOT NULL,
    [LogoffTimeStamp] DATETIME2 NOT NULL,
    [OperatingSystem] NVARCHAR(50) NOT NULL,
    [ClientSoftware] NVARCHAR(50) NOT NULL,
    [RemoteIpAddress] NVARCHAR(50) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2,
    CONSTRAINT [PK_SessionLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create PaymentStrategyPlan table to store information about each user's payment strategy
CREATE TABLE [PaymentStrategyPlan] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [StrategyTypeId] INT NOT NULL,
    [GlobalMonthlyPayment] DECIMAL(10,3) NOT NULL, -- The total amount the user is able to pay across all loans each month
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2,
    CONSTRAINT [PK_PaymentStrategyPlan] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create Loan table to store information about each loan
CREATE TABLE [Loan] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [LoanNickName] NVARCHAR(50) NOT NULL,
    [PaymentStrategy] INT NOT NULL,
    [Principal] DECIMAL(10,3) NOT NULL,
    [InterestRate] DECIMAL(5,5) NOT NULL,
    [Fees] DECIMAL(10,3) NOT NULL,
    [MonthlyPayment] DECIMAL(10,3) NOT NULL,
    [RemainingTerm] INT NOT NULL,
    [Currency] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2,
    CONSTRAINT [PK_Loan] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_Loan_LoanNickName] UNIQUE ([LoanNickName]) -- Ensure that each loan nickname is unique
);

-- Create Currency table to store different types of currencies
CREATE TABLE [Currency] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FormalName] NVARCHAR(50) NOT NULL,
    [ShortName] NVARCHAR(20) NOT NULL,
    [Symbol] NVARCHAR(10) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2,
    CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create DebtSnowflake table to store information about extra payments on the debts
CREATE TABLE [DebtSnowflake] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Date] DATE NOT NULL,
    [Amount] DECIMAL(10,3) NOT NULL,
    [PaymentStrategy] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2,
    CONSTRAINT [PK_DebtSnowflake] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create StrategyType table to store different types of payment strategies
CREATE TABLE [StrategyType] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Type] NVARCHAR(20) NOT NULL,
    [HasCustomStrategy] BIT NOT NULL, -- Indicates whether the strategy is custom or not
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2,
    CONSTRAINT [PK_StrategyType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create LoanCardinalOrder table to store the order in which loans should be paid off
CREATE TABLE [LoanCardinalOrder] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [PaymentStrategy] INT NOT NULL,
    [LoanId] INT NOT NULL,
    [CardinalOrder] INT NOT NULL, -- The order in which the loan should be paid off
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2,
    CONSTRAINT [PK_LoanCardinalOrder] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Add foreign key constraints to ensure data integrity
ALTER TABLE [User] ADD CONSTRAINT [FK_User_UserTypeId_UserType_Id] FOREIGN KEY([UserTypeId])
    REFERENCES [UserType] ([Id]);

ALTER TABLE [SessionLog] ADD CONSTRAINT [FK_SessionLog_UserId_User_Id] FOREIGN KEY([UserId])
    REFERENCES [User] ([Id]);

ALTER TABLE [PaymentStrategyPlan] ADD CONSTRAINT [FK_PaymentStrategyPlan_UserId_User_Id] FOREIGN KEY([UserId])
    REFERENCES [User] ([Id]);

ALTER TABLE [PaymentStrategyPlan] ADD CONSTRAINT [FK_PaymentStrategyPlan_StrategyTypeId_StrategyType_Id] FOREIGN KEY([StrategyTypeId])
    REFERENCES [StrategyType] ([Id]);

ALTER TABLE [Loan] ADD CONSTRAINT [FK_Loan_PaymentStrategy_PaymentStrategyPlan_Id] FOREIGN KEY([PaymentStrategy])
    REFERENCES [PaymentStrategyPlan] ([Id]);

ALTER TABLE [Loan] ADD CONSTRAINT [FK_Loan_Currency_Currency_Id] FOREIGN KEY([Currency])
    REFERENCES [Currency] ([Id]);

ALTER TABLE [DebtSnowflake] ADD CONSTRAINT [FK_DebtSnowflake_PaymentStrategy_PaymentStrategyPlan_Id] FOREIGN KEY([PaymentStrategy])
    REFERENCES [PaymentStrategyPlan] ([Id]);

ALTER TABLE [LoanCardinalOrder] ADD CONSTRAINT [FK_LoanCardinalOrder_PaymentStrategy_PaymentStrategyPlan_Id] FOREIGN KEY([PaymentStrategy])
    REFERENCES [PaymentStrategyPlan] ([Id]);

ALTER TABLE [LoanCardinalOrder] ADD CONSTRAINT [FK_LoanCardinalOrder_LoanId_Loan_Id]

ALTER TABLE [LoanCardinalOrder] ADD CONSTRAINT [FK_LoanCardinalOrder_LoanId_Loan_Id] FOREIGN KEY([LoanId])
    REFERENCES [Loan] ([Id]);
