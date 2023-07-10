-- Create User table to store user information
CREATE TABLE `User` (
                        `Id` INT  NOT NULL,
                        `FirstName` VARCHAR(50)  NOT NULL,
                        `LastName` VARCHAR(50)  NOT NULL,
                        `Email` NVARCHAR(MAX)  NOT NULL,
                        `Password` VARCHAR(255)  NOT NULL, -- This should store hashed and salted passwords, not plain text
                        `UserTypeId` INT  NOT NULL,
                        `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, -- Timestamp of when the record was created
                        `UpdatedAt` DATETIME, -- Timestamp of the last update to the record
                        PRIMARY KEY (`Id`),
                        CONSTRAINT `uc_User_Email` UNIQUE (`Email`) -- Ensure that each email is unique
);

-- Create UserType table to store different types of users
CREATE TABLE `UserType` (
                            `Id` INT  NOT NULL,
                            `Type` VARCHAR(20)  NOT NULL,
                            `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            `UpdatedAt` DATETIME,
                            PRIMARY KEY (`Id`)
);

-- Create SessionLog table to store user session information
CREATE TABLE `SessionLog` (
                              `Id` INT  NOT NULL,
                              `UserId` INT  NOT NULL,
                              `LogonTimeStamp` DATETIME  NOT NULL,
                              `LogoffTimeStamp` DATETIME  NOT NULL,
                              `OperatingSystem` VARCHAR(50)  NOT NULL,
                              `ClientSoftware` VARCHAR(50)  NOT NULL,
                              `RemoteIpAddress` VARCHAR(50)  NOT NULL,
                              `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `UpdatedAt` DATETIME,
                              PRIMARY KEY (`Id`)
);

-- Create PaymentStrategyPlan table to store information about each user's payment strategy
CREATE TABLE `PaymentStrategyPlan` (
                                       `Id` INT  NOT NULL,
                                       `UserId` INT  NOT NULL,
                                       `StrategyTypeId` INT  NOT NULL,
                                       `GlobalMonthlyPayment` DECIMAL(10,3)  NOT NULL, -- The total amount the user is able to pay across all loans each month
                                       `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                       `UpdatedAt` DATETIME,
                                       PRIMARY KEY (`Id`)
);

-- Create Loan table to store information about each loan
CREATE TABLE `Loan` (
                        `Id` INT  NOT NULL,
                        `LoanNickName` VARCHAR(50)  NOT NULL,
                        `PaymentStrategy` INT  NOT NULL,
                        `Principal` DECIMAL(10,3)  NOT NULL,
                        `InterestRate` DECIMAL(5,5)  NOT NULL,
                        `Fees` DECIMAL(10,3)  NOT NULL,
                        `MonthlyPayment` DECIMAL(10,3) NOT NULL,
                        `RemainingTerm` INT  NOT NULL,
                        `Currency` INT  NOT NULL,
                        `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        `UpdatedAt` DATETIME,
                        PRIMARY KEY (`Id`),
                        CONSTRAINT `uc_Loan_LoanNickName` UNIQUE (`LoanNickName`) -- Ensure that each loan nickname is unique
);

-- Create Currency table to store different types of currencies
CREATE TABLE `Currency` (
                            `Id` INT  NOT NULL,
                            `FormalName` VARCHAR(50)  NOT NULL,
                            `ShortName` VARCHAR(20)  NOT NULL,
                            `Symbol` VARCHAR(10)  NOT NULL,
                            `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            `UpdatedAt` DATETIME,
                            PRIMARY KEY (`Id`)
);

-- Create DebtSnowflake table to store information about extra payments on the debts
CREATE TABLE `DebtSnowflake` (
                                 `Id` INT  NOT NULL,
                                 `Date` DATE  NOT NULL,
                                 `Amount` DECIMAL(10,3)  NOT NULL,
                                 `PaymentStrategy` INT  NOT NULL,
                                 `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                 `UpdatedAt` DATETIME,
                                 PRIMARY KEY (`Id`)
);

-- Create StrategyType table to store different types of payment strategies
CREATE TABLE `StrategyType` (
                                `Id` INT  NOT NULL,
                                `Type` VARCHAR(20)  NOT NULL,
                                `HasCustomStrategy` BIT  NOT NULL, -- Indicates whether the strategy is custom or not
                                `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                `UpdatedAt` DATETIME,
                                PRIMARY KEY (`Id`)
);

-- Create LoanCardinalOrder table to store the order in which loans should be paid off
CREATE TABLE `LoanCardinalOrder` (
                                     `Id` INT  NOT NULL,
                                     `PaymentStrategy` INT  NOT NULL,
                                     `LoanId` INT  NOT NULL,
                                     `CardinalOrder` INT  NOT NULL, -- The order in which the loan should be paid off
                                     `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                     `UpdatedAt` DATETIME,
                                     PRIMARY KEY (`Id`)
);

-- Add foreign key constraints to ensure data integrity
ALTER TABLE `User` ADD CONSTRAINT `fk_User_UserTypeId_UserType_Id` FOREIGN KEY(`UserTypeId`)
    REFERENCES `UserType` (`Id`);

ALTER TABLE `SessionLog` ADD CONSTRAINT `fk_SessionLog_UserId_User_Id` FOREIGN KEY(`UserId`)
    REFERENCES `User` (`Id`);

ALTER TABLE `PaymentStrategyPlan` ADD CONSTRAINT `fk_PaymentStrategyPlan_UserId_User_Id` FOREIGN KEY(`UserId`)
    REFERENCES `User` (`Id`);

ALTER TABLE `PaymentStrategyPlan` ADD CONSTRAINT `fk_PaymentStrategyPlan_StrategyTypeId_StrategyType_Id` FOREIGN KEY(`StrategyTypeId`)
    REFERENCES `StrategyType` (`Id`);

ALTER TABLE `Loan` ADD CONSTRAINT `fk_Loan_PaymentStrategy_PaymentStrategyPlan_Id` FOREIGN KEY(`PaymentStrategy`)
    REFERENCES `PaymentStrategyPlan` (`Id`);

ALTER TABLE `Loan` ADD CONSTRAINT `fk_Loan_Currency_Currency_Id` FOREIGN KEY(`Currency`)
    REFERENCES `Currency` (`Id`);

ALTER TABLE `DebtSnowflake` ADD CONSTRAINT `fk_DebtSnowflake_PaymentStrategy_PaymentStrategyPlan_Id` FOREIGN KEY(`PaymentStrategy`)
    REFERENCES `PaymentStrategyPlan` (`Id`);

ALTER TABLE `LoanCardinalOrder` ADD CONSTRAINT `fk_LoanCardinalOrder_PaymentStrategy_PaymentStrategyPlan_Id` FOREIGN KEY(`PaymentStrategy`)
    REFERENCES `PaymentStrategyPlan` (`Id`);

ALTER TABLE `LoanCardinalOrder` ADD CONSTRAINT `fk_LoanCardinalOrder_LoanId_Loan_Id` FOREIGN KEY(`LoanId`)
    REFERENCES `Loan` (`Id`);
