/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2008                    */
/* Created on:     10/06/2025 12:07:11 SA                       */
/*==============================================================*/

CREATE DATABASE GYM
GO
USE GYM
GO

-- Drop foreign key constraints
IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('MemberPakage') AND o.name = 'FK_MEMBERPA_MEMBERPAK_MEMBER')
    ALTER TABLE MemberPakage
        DROP CONSTRAINT FK_MEMBERPA_MEMBERPAK_MEMBER
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('MemberPakage') AND o.name = 'FK_MEMBERPA_MEMBERPAK_PACKAGE')
    ALTER TABLE MemberPakage
        DROP CONSTRAINT FK_MEMBERPA_MEMBERPAK_PACKAGE
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('MemberPayment') AND o.name = 'FK_MEMBERPA_MEMBERPAY_MEMBER')
    ALTER TABLE MemberPayment
        DROP CONSTRAINT FK_MEMBERPA_MEMBERPAY_MEMBER
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('MemberPayment') AND o.name = 'FK_MEMBERPA_MEMBERPAY_PAYMENT')
    ALTER TABLE MemberPayment
        DROP CONSTRAINT FK_MEMBERPA_MEMBERPAY_PAYMENT
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('MemberPayment') AND o.name = 'FK_MEMBERPA_MEMBERPAY_STAFF')
    ALTER TABLE MemberPayment
        DROP CONSTRAINT FK_MEMBERPA_MEMBERPAY_STAFF
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('TrainingSchedule') AND o.name = 'FK_TRAINING_TRAININGS_TRAINER')
    ALTER TABLE TrainingSchedule
        DROP CONSTRAINT FK_TRAINING_TRAININGS_TRAINER
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('TrainingSchedule') AND o.name = 'FK_TRAINING_TRAININGS_MEMBER')
    ALTER TABLE TrainingSchedule
        DROP CONSTRAINT FK_TRAINING_TRAININGS_MEMBER
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('"User"') AND o.name = 'FK_USER_INCLUDE_ROLE')
    ALTER TABLE "User"
        DROP CONSTRAINT FK_USER_INCLUDE_ROLE
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('UserNotification') AND o.name = 'FK_USERNOTI_USERNOTIF_NOTIFICA')
    ALTER TABLE UserNotification
        DROP CONSTRAINT FK_USERNOTI_USERNOTIF_NOTIFICA
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('UserNotification') AND o.name = 'FK_USERNOTI_USERNOTIF_USER')
    ALTER TABLE UserNotification
        DROP CONSTRAINT FK_USERNOTI_USERNOTIF_USER
GO

-- Drop tables
IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Member') AND type = 'U')
    DROP TABLE Member
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('MemberPakage') AND name = 'MemberPakage2_FK' AND indid > 0 AND indid < 255)
    DROP INDEX MemberPakage.MemberPakage2_FK
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('MemberPakage') AND name = 'MemberPakage_FK' AND indid > 0 AND indid < 255)
    DROP INDEX MemberPakage.MemberPakage_FK
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('MemberPakage') AND type = 'U')
    DROP TABLE MemberPakage
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('MemberPayment') AND name = 'MemberPayment3_FK' AND indid > 0 AND indid < 255)
    DROP INDEX MemberPayment.MemberPayment3_FK
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('MemberPayment') AND name = 'MemberPayment2_FK' AND indid > 0 AND indid < 255)
    DROP INDEX MemberPayment.MemberPayment2_FK
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('MemberPayment') AND name = 'MemberPayment_FK' AND indid > 0 AND indid < 255)
    DROP INDEX MemberPayment.MemberPayment_FK
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('MemberPayment') AND type = 'U')
    DROP TABLE MemberPayment
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Notification') AND type = 'U')
    DROP TABLE Notification
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Package') AND type = 'U')
    DROP TABLE Package
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Payment') AND type = 'U')
    DROP TABLE Payment
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Role') AND type = 'U')
    DROP TABLE Role
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Staff') AND type = 'U')
    DROP TABLE Staff
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Trainer') AND type = 'U')
    DROP TABLE Trainer
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('TrainingSchedule') AND name = 'TrainingSchedule2_FK' AND indid > 0 AND indid < 255)
    DROP INDEX TrainingSchedule.TrainingSchedule2_FK
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('TrainingSchedule') AND name = 'TrainingSchedule_FK' AND indid > 0 AND indid < 255)
    DROP INDEX TrainingSchedule.TrainingSchedule_FK
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('TrainingSchedule') AND type = 'U')
    DROP TABLE TrainingSchedule
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('"User"') AND name = 'include_FK' AND indid > 0 AND indid < 255)
    DROP INDEX "User".include_FK
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('"User"') AND type = 'U')
    DROP TABLE "User"
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('UserNotification') AND name = 'UserNotification2_FK' AND indid > 0 AND indid < 255)
    DROP INDEX UserNotification.UserNotification2_FK
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('UserNotification') AND name = 'UserNotification_FK' AND indid > 0 AND indid < 255)
    DROP INDEX UserNotification.UserNotification_FK
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('UserNotification') AND type = 'U')
    DROP TABLE UserNotification
GO

/*==============================================================*/
/* Table: Member                                                */
/*==============================================================*/
CREATE TABLE Member (
    memberId INT NOT NULL IDENTITY(1,1),
    fullName NVARCHAR(255) NULL,
    dateOfBirth DATETIME NULL,
    sex BIT NULL,
    phone CHAR(10) NULL,
    address NVARCHAR(255) NULL,
    createDate DATETIME NULL,
    CONSTRAINT PK_MEMBER PRIMARY KEY NONCLUSTERED (memberId)
)
GO

/*==============================================================*/
/* Table: MemberPakage                                          */
/*==============================================================*/
CREATE TABLE MemberPakage (
    memberId INT NOT NULL,
    packageId INT NOT NULL,
    startDate DATETIME NULL,
    endDate DATETIME NULL,
    isPaid BIT NULL,
    isActive BIT NULL,
    CONSTRAINT PK_MEMBERPAKAGE PRIMARY KEY (memberId, packageId)
)
GO

/*==============================================================*/
/* Index: MemberPakage_FK                                       */
/*==============================================================*/
CREATE INDEX MemberPakage_FK ON MemberPakage (
    memberId ASC
)
GO

/*==============================================================*/
/* Index: MemberPakage2_FK                                      */
/*==============================================================*/
CREATE INDEX MemberPakage2_FK ON MemberPakage (
    packageId ASC
)
GO

/*==============================================================*/
/* Table: MemberPayment                                         */
/*==============================================================*/
CREATE TABLE MemberPayment (
    memberId INT NOT NULL,
    paymentId INT NOT NULL,
    staffId INT NOT NULL,
    paymentDate DATETIME NULL,
    CONSTRAINT PK_MEMBERPAYMENT PRIMARY KEY (memberId, paymentId, staffId)
)
GO

/*==============================================================*/
/* Index: MemberPayment_FK                                      */
/*==============================================================*/
CREATE INDEX MemberPayment_FK ON MemberPayment (
    memberId ASC
)
GO

/*==============================================================*/
/* Index: MemberPayment2_FK                                     */
/*==============================================================*/
CREATE INDEX MemberPayment2_FK ON MemberPayment (
    paymentId ASC
)
GO

/*==============================================================*/
/* Index: MemberPayment3_FK                                     */
/*==============================================================*/
CREATE INDEX MemberPayment3_FK ON MemberPayment (
    staffId ASC
)
GO

/*==============================================================*/
/* Table: Notification                                          */
/*==============================================================*/
CREATE TABLE Notification (
    notificationId INT NOT NULL IDENTITY(1,1),
    title NVARCHAR(255) NULL,
    content NVARCHAR(255) NULL,
    createdAt DATETIME NULL,
    sendRole INT NULL,
    CONSTRAINT PK_NOTIFICATION PRIMARY KEY NONCLUSTERED (notificationId)
)
GO

/*==============================================================*/
/* Table: Package                                               */
/*==============================================================*/
CREATE TABLE Package (
    packageId INT NOT NULL IDENTITY(1,1),
    name NVARCHAR(255) NULL,
    type NVARCHAR(100) NULL,
    price DECIMAL(10,2) NULL,
    durationInDays INT NULL,
    description NVARCHAR(255) NULL,
    CONSTRAINT PK_PACKAGE PRIMARY KEY NONCLUSTERED (packageId)
)
GO

/*==============================================================*/
/* Table: Payment                                               */
/*==============================================================*/
CREATE TABLE Payment (
    paymentId INT NOT NULL IDENTITY(1,1),
    amount DECIMAL(10,2) NULL,
    method NVARCHAR(100) NULL,
    CONSTRAINT PK_PAYMENT PRIMARY KEY NONCLUSTERED (paymentId)
)
GO

/*==============================================================*/
/* Table: Role                                                  */
/*==============================================================*/
CREATE TABLE Role (
    roleId INT NOT NULL IDENTITY(1,1),
    roleName NVARCHAR(255) NULL,
    description NVARCHAR(255) NULL,
    CONSTRAINT PK_ROLE PRIMARY KEY NONCLUSTERED (roleId)
)
GO

/*==============================================================*/
/* Table: Staff                                                 */
/*==============================================================*/
CREATE TABLE Staff (
    staffId INT NOT NULL IDENTITY(1,1),
    fullName NVARCHAR(255) NULL,
    phone CHAR(10) NULL,
    email NVARCHAR(100) NULL,
    workingSince DATETIME NULL,
    CONSTRAINT PK_STAFF PRIMARY KEY NONCLUSTERED (staffId)
)
GO

/*==============================================================*/
/* Table: Trainer                                               */
/*==============================================================*/
CREATE TABLE Trainer (
    trainerId INT NOT NULL IDENTITY(1,1),
    fullName NVARCHAR(255) NULL,
    phone CHAR(10) NULL,
    specialty NVARCHAR(255) NULL,
    scheduleNote NVARCHAR(255) NULL,
    CONSTRAINT PK_TRAINER PRIMARY KEY NONCLUSTERED (trainerId)
)
GO

/*==============================================================*/
/* Table: TrainingSchedule                                      */
/*==============================================================*/
CREATE TABLE TrainingSchedule (
    trainerId INT NOT NULL,
    memberId INT NOT NULL,
    trainingDate DATETIME NULL,
    startTime DATETIME NULL,
    endTime DATETIME NULL,
    node NVARCHAR(255) NULL,
    CONSTRAINT PK_TRAININGSCHEDULE PRIMARY KEY (trainerId, memberId)
)
GO

/*==============================================================*/
/* Index: TrainingSchedule_FK                                   */
/*==============================================================*/
CREATE INDEX TrainingSchedule_FK ON TrainingSchedule (
    trainerId ASC
)
GO

/*==============================================================*/
/* Index: TreatmentSchedule2_FK                                  */
/*==============================================================*/
CREATE INDEX TrainingSchedule2_FK ON TrainingSchedule (
    memberId ASC
)
GO

/*==============================================================*/
/* Table: "User"                                                */
/*==============================================================*/
CREATE TABLE "User" (
    userId INT NOT NULL IDENTITY(1,1),
    roleId INT NOT NULL,
    userName NVARCHAR(100) NULL,
    password NVARCHAR(100) NULL,
    email NVARCHAR(100) NULL,
    referenceId INT NULL,
    status NVARCHAR(255) NULL,
    isAtive BIT NULL,
    CONSTRAINT PK_USER PRIMARY KEY NONCLUSTERED (userId)
)
GO

/*==============================================================*/
/* Table: "Feedback"                                                */
/*==============================================================*/
CREATE TABLE Feedback (
    feedbackId INT IDENTITY(1,1) PRIMARY KEY,
    userId INT NOT NULL,
    content NVARCHAR(255) NULL,
    thoiGian DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Feedback_User FOREIGN KEY (userId) REFERENCES [User](userId)
)
GO

/*==============================================================*/
/* Index: include_FK                                            */
/*==============================================================*/
CREATE INDEX include_FK ON "User" (
    roleId ASC
)
GO

/*==============================================================*/
/* Table: UserNotification                                      */
/*==============================================================*/
CREATE TABLE UserNotification (
    notificationId INT NOT NULL,
    userId INT NOT NULL,
    timeSend DATETIME NULL,
    seen BIT NULL,
    CONSTRAINT PK_USERNOTIFICATION PRIMARY KEY (notificationId, userId)
)
GO

/*==============================================================*/
/* Index: UserNotification_FK                                   */
/*==============================================================*/
CREATE INDEX UserNotification_FK ON UserNotification (
    notificationId ASC
)
GO

/*==============================================================*/
/* Index: UserNotification2_FK                                  */
/*==============================================================*/
CREATE INDEX UserNotification2_FK ON UserNotification (
    userId ASC
)
GO

-- Add foreign key constraints
ALTER TABLE MemberPakage
    ADD CONSTRAINT FK_MEMBERPA_MEMBERPAK_MEMBER FOREIGN KEY (memberId)
        REFERENCES Member (memberId)
GO

ALTER TABLE MemberPakage
    ADD CONSTRAINT FK_MEMBERPA_MEMBERPAK_PACKAGE FOREIGN KEY (packageId)
        REFERENCES Package (packageId)
GO

ALTER TABLE MemberPayment
    ADD CONSTRAINT FK_MEMBERPA_MEMBERPAY_MEMBER FOREIGN KEY (memberId)
        REFERENCES Member (memberId)
GO

ALTER TABLE MemberPayment
    ADD CONSTRAINT FK_MEMBERPA_MEMBERPAY_PAYMENT FOREIGN KEY (paymentId)
        REFERENCES Payment (paymentId)
GO

ALTER TABLE MemberPayment
    ADD CONSTRAINT FK_MEMBERPA_MEMBERPAY_STAFF FOREIGN KEY (staffId)
        REFERENCES Staff (staffId)
GO

ALTER TABLE TrainingSchedule
    ADD CONSTRAINT FK_TRAINING_TRAININGS_TRAINER FOREIGN KEY (trainerId)
        REFERENCES Trainer (trainerId)
GO

ALTER TABLE TrainingSchedule
    ADD CONSTRAINT FK_TRAINING_TRAININGS_MEMBER FOREIGN KEY (memberId)
        REFERENCES Member (memberId)
GO

ALTER TABLE "User"
    ADD CONSTRAINT FK_USER_INCLUDE_ROLE FOREIGN KEY (roleId)
        REFERENCES Role (roleId)
GO

ALTER TABLE UserNotification
    ADD CONSTRAINT FK_USERNOTI_USERNOTIF_NOTIFICA FOREIGN KEY (notificationId)
        REFERENCES Notification (notificationId)
GO

ALTER TABLE UserNotification
    ADD CONSTRAINT FK_USERNOTI_USERNOTIF_USER FOREIGN KEY (userId)
        REFERENCES "User" (userId)
GO

/*==============================================================*/
/*                            INSERT                            */
/*==============================================================*/


INSERT INTO [GYM].[dbo].[Role] ([roleName], [description])
VALUES 
    ('Admin', N'Người quản trị hệ thống, có toàn quyền quản lý'),
    ('Member', N'Hội viên sử dụng dịch vụ phòng gym'),
    ('Staff', N'Nhân viên hỗ trợ và chăm sóc khách hàng'),
    ('Trainer', N'Huấn luyện viên hướng dẫn hội viên');
go

INSERT INTO [GYM].[dbo].[User] ([roleId], [userName], [password], [email], [referenceId], [status], [isAtive])
VALUES 
    -- Admin
    (1, N'admin', '$2y$10$3jeviuH6Nf.xKkqe3peQMeydzoILnoCa/LljFvZTiJPAzayhrkfue', N'nguyen.admin@gym.com', NULL, N'Hoạt động', 1), -- pass: admin
    -- Members pass=1
    (2, N'nguyenvanan', '$2y$10$RxDmO1pLo.adx9dJ.dhZV.PPOkchXpKe0UB79E9Koact204jdFZqq', N'nguyenvanan@gmail.com', 1, N'Hoạt động', 1),
    (2, N'tranthiminh', '$2y$10$RxDmO1pLo.adx9dJ.dhZV.PPOkchXpKe0UB79E9Koact204jdFZqq', N'tranthiminh@gmail.com', 2, N'Hoạt động', 1),
    (2, N'phamvanquang', '$2y$10$RxDmO1pLo.adx9dJ.dhZV.PPOkchXpKe0UB79E9Koact204jdFZqq', N'phamvanquang@gmail.com', 3, N'Hoạt động', 1),
    (2, N'lethithuy', '$2y$10$RxDmO1pLo.adx9dJ.dhZV.PPOkchXpKe0UB79E9Koact204jdFZqq', N'lethithuy@gmail.com', 4, N'Hoạt động', 1),
    (2, N'hovanlong', '$2y$10$RxDmO1pLo.adx9dJ.dhZV.PPOkchXpKe0UB79E9Koact204jdFZqq', N'hovanlong@gmail.com', 5, N'Hoạt động', 1),
    (2, N'vuongthihuong', '$2y$10$RxDmO1pLo.adx9dJ.dhZV.PPOkchXpKe0UB79E9Koact204jdFZqq', N'vuongthihuong@gmail.com', 6, N'Hoạt động', 1),
    (2, N'dangvanduc', '$2y$10$RxDmO1pLo.adx9dJ.dhZV.PPOkchXpKe0UB79E9Koact204jdFZqq', N'dangvanduc@gmail.com', 7, N'Hoạt động', 1),
    (2, N'dothilan', '$2y$10$RxDmO1pLo.adx9dJ.dhZV.PPOkchXpKe0UB79E9Koact204jdFZqq', N'dothilan@gmail.com', 8, N'Hoạt động', 1),
    (2, N'trunghoang', '$2y$10$RxDmO1pLo.adx9dJ.dhZV.PPOkchXpKe0UB79E9Koact204jdFZqq', N'trunghoang@gmail.com', 9, N'Hoạt động', 1),
    (2, N'buithiphuong', '$2y$10$RxDmO1pLo.adx9dJ.dhZV.PPOkchXpKe0UB79E9Koact204jdFZqq', N'buithiphuong@gmail.com', 10, N'Hoạt động', 1),
    -- Staff pass=2
    (3, N'nguyenhoangnam', '$2y$10$3ARFYYBFnrAdY6KxsVmeuezkaUvffFfYpIGhu1Qtebzqt4t6uuOUu', N'nguyenhoangnam@gmail.com', 1, N'Hoạt động', 1),
    (3, N'tranthihong', '$2y$10$3ARFYYBFnrAdY6KxsVmeuezkaUvffFfYpIGhu1Qtebzqt4t6uuOUu', N'tranthihong@gmail.com', 2, N'Hoạt động', 1),
    (3, N'phamducthanh', '$2y$10$3ARFYYBFnrAdY6KxsVmeuezkaUvffFfYpIGhu1Qtebzqt4t6uuOUu', N'phamducthanh@gmail.com', 3, N'Hoạt động', 1),
    (3, N'leminhhieu', '$2y$10$3ARFYYBFnrAdY6KxsVmeuezkaUvffFfYpIGhu1Qtebzqt4t6uuOUu', N'leminhhieu@gmail.com', 4, N'Hoạt động', 1),
    (3, N'hothithanh', '$2y$10$3ARFYYBFnrAdY6KxsVmeuezkaUvffFfYpIGhu1Qtebzqt4t6uuOUu', N'hothithanh@gmail.com', 5, N'Hoạt động', 1),
    (3, N'vuquocbao', '$2y$10$3ARFYYBFnrAdY6KxsVmeuezkaUvffFfYpIGhu1Qtebzqt4t6uuOUu', N'vuquocbao@gmail.com', 6, N'Hoạt động', 1),
    (3, N'dangthimai', '$2y$10$3ARFYYBFnrAdY6KxsVmeuezkaUvffFfYpIGhu1Qtebzqt4t6uuOUu', N'dangthimai@gmail.com', 7, N'Hoạt động', 1),
    (3, N'dovanthang', '$2y$10$3ARFYYBFnrAdY6KxsVmeuezkaUvffFfYpIGhu1Qtebzqt4t6uuOUu', N'dovanthang@gmail.com', 8, N'Hoạt động', 1),
    (3, N'trinhthihuong', '$2y$10$3ARFYYBFnrAdY6KxsVmeuezkaUvffFfYpIGhu1Qtebzqt4t6uuOUu', N'trinhthihuong@gmail.com', 9, N'Hoạt động', 1),
    (3, N'buiduchai', '$2y$10$3ARFYYBFnrAdY6KxsVmeuezkaUvffFfYpIGhu1Qtebzqt4t6uuOUu', N'buiduchai@gmail.com', 10, N'Hoạt động', 1),
    -- Trainers pass=3
    (4, N'nguyenvanloc', '$2a$12$Ib.I1uVGS6Dbv26wy24vSOEjOFKN9jjgVtx1.XuxPTMlU1Nh6y6yS', N'nguyenvanloc@gmail.com', 1, N'Hoạt động', 1),
    (4, N'tranthilinh', '$2a$12$Ib.I1uVGS6Dbv26wy24vSOEjOFKN9jjgVtx1.XuxPTMlU1Nh6y6yS', N'tranthilinh@gmail.com', 2, N'Hoạt động', 1),
    (4, N'phamvanthien', '$2a$12$Ib.I1uVGS6Dbv26wy24vSOEjOFKN9jjgVtx1.XuxPTMlU1Nh6y6yS', N'phamvanthien@gmail.com', 3, N'Hoạt động', 1),
    (4, N'lethithao', '$2a$12$Ib.I1uVGS6Dbv26wy24vSOEjOFKN9jjgVtx1.XuxPTMlU1Nh6y6yS', N'lethithao@gmail.com', 4, N'Hoạt động', 1),
    (4, N'hovankhoi', '$2a$12$Ib.I1uVGS6Dbv26wy24vSOEjOFKN9jjgVtx1.XuxPTMlU1Nh6y6yS', N'hovankhoi@gmail.com', 5, N'Hoạt động', 1),
    (4, N'vuongductuan', '$2a$12$Ib.I1uVGS6Dbv26wy24vSOEjOFKN9jjgVtx1.XuxPTMlU1Nh6y6yS', N'vuongductuan@gmail.com', 6, N'Hoạt động', 1),
    (4, N'dangthian', '$2a$12$Ib.I1uVGS6Dbv26wy24vSOEjOFKN9jjgVtx1.XuxPTMlU1Nh6y6yS', N'dangthian@gmail.com', 7, N'Hoạt động', 1),
    (4, N'dothithuy', '$2a$12$Ib.I1uVGS6Dbv26wy24vSOEjOFKN9jjgVtx1.XuxPTMlU1Nh6y6yS', N'dothithuy@gmail.com', 8, N'Hoạt động', 1),
    (4, N'trunghoangphi', '$2a$12$Ib.I1uVGS6Dbv26wy24vSOEjOFKN9jjgVtx1.XuxPTMlU1Nh6y6yS', N'trunghoangphi@gmail.com', 9, N'Hoạt động', 1),
    (4, N'buithiphuong', '$2a$12$Ib.I1uVGS6Dbv26wy24vSOEjOFKN9jjgVtx1.XuxPTMlU1Nh6y6yS', N'buithiphuong@gmail.com', 10, N'Hoạt động', 1);

go
INSERT INTO [GYM].[dbo].[Member] ([fullName], [dateOfBirth], [sex], [phone], [address], [createDate])
VALUES 
    (N'Nguyễn Văn An', '1995-03-15 00:00:00', 1, '0912345678', N'123 Đường Láng, Hà Nội', '2025-06-01 00:00:00'),
    (N'Trần Thị Minh', '1998-07-22 00:00:00', 0, '0987654321', N'456 Cầu Giấy, Hà Nội', '2025-06-02 00:00:00'),
    (N'Phạm Văn Quang', '1993-11-10 00:00:00', 1, '0901234567', N'789 Nguyễn Trãi, Hà Nội', '2025-06-03 00:00:00'),
    (N'Lê Thị Thủy', '1997-05-18 00:00:00', 0, '0923456789', N'101 Hoàng Cầu, Hà Nội', '2025-06-04 00:00:00'),
    (N'Hồ Văn Long', '1994-09-25 00:00:00', 1, '0934567890', N'234 Kim Mã, Hà Nội', '2025-06-05 00:00:00'),
    (N'Vương Thị Hương', '1999-02-14 00:00:00', 0, '0945678901', N'567 Giải Phóng, Hà Nội', '2025-06-06 00:00:00'),
    (N'Đặng Văn Đức', '1992-12-30 00:00:00', 1, '0956789012', N'890 Tây Sơn, Hà Nội', '2025-06-07 00:00:00'),
    (N'Đỗ Thị Lan', '1996-04-05 00:00:00', 0, '0967890123', N'111 Láng Hạ, Hà Nội', '2025-06-08 00:00:00'),
    (N'Trương Hoàng', '1995-08-20 00:00:00', 1, '0978901234', N'222 Thanh Nhàn, Hà Nội', '2025-06-09 00:00:00'),
    (N'Bùi Thị Phương', '1998-01-12 00:00:00', 0, '0989012345', N'333 Lê Duẩn, Hà Nội', '2025-06-10 00:00:00');

go
INSERT INTO [GYM].[dbo].[Staff] ([fullName], [phone], [email], [workingSince])
VALUES 
    (N'Nguyễn Hoàng Nam', '0901234567', N'nguyenhoangnam@gmail.com', '2024-01-10 00:00:00'),
    (N'Trần Thị Hồng', '0923456789', N'tranthihong@gmail.com', '2024-02-15 00:00:00'),
    (N'Phạm Đức Thành', '0934567890', N'phamducthanh@gmail.com', '2024-03-20 00:00:00'),
    (N'Lê Minh Hiếu', '0945678901', N'leminhhieu@gmail.com', '2024-04-25 00:00:00'),
    (N'Hồ Thị Thanh', '0956789012', N'hothithanh@gmail.com', '2024-05-30 00:00:00'),
    (N'Vũ Quốc Bảo', '0967890123', N'vuquocbao@gmail.com', '2024-06-05 00:00:00'),
    (N'Đặng Thị Mai', '0978901234', N'dangthimai@gmail.com', '2024-07-10 00:00:00'),
    (N'Đỗ Văn Thắng', '0989012345', N'dovanthang@gmail.com', '2024-08-15 00:00:00'),
    (N'Trịnh Thị Hương', '0990123456', N'trinhthihuong@gmail.com', '2024-09-20 00:00:00'),
    (N'Bùi Đức Hải', '0911234567', N'buiduchai@gmail.com', '2024-10-25 00:00:00');

go
INSERT INTO [GYM].[dbo].[Trainer] ([fullName], [phone], [specialty], [scheduleNote])
VALUES 
    (N'Nguyễn Văn Lộc', '0902345678', N'Tập gym tăng cơ', N'Thứ 2-6: 8h-12h, 14h-18h'),
    (N'Trần Thị Linh', '0924567890', N'Yoga và thiền', N'Thứ 3-7: 7h-11h, 15h-19h'),
    (N'Phạm Văn Thiên', '0935678901', N'Cardio và sức bền', N'Thứ 2-6: 9h-13h, 16h-20h'),
    (N'Lê Thị Thảo', '0946789012', N'Pilates', N'Thứ 4-7: 8h-12h, 14h-18h'),
    (N'Hồ Văn Khôi', '0957890123', N'Tập gym giảm cân', N'Thứ 2-5: 7h-11h, 15h-19h'),
    (N'Vương Đức Tuấn', '0968901234', N'CrossFit', N'Thứ 3-6: 8h-12h, 14h-18h'),
    (N'Đặng Thị An', '0979012345', N'Zumba', N'Thứ 2-6: 17h-21h'),
    (N'Đỗ Thị Thủy', '0980123456', N'Tập gym tăng cơ', N'Thứ 3-7: 7h-11h, 15h-19h'),
    (N'Trương Hoàng Phi', '0991234567', N'Kickboxing', N'Thứ 2-6: 8h-12h, 14h-18h'),
    (N'Bùi Thị Phương', '0912345679', N'Yoga nâng cao', N'Thứ 4-7: 7h-11h, 15h-19h');
go

INSERT INTO [GYM].[dbo].[Package] ([name], [type], [price], [durationInDays], [description])
VALUES 
    (N'Gói 1 Ngày', N'Tiêu chuẩn', 30000.00, 1, N'Truy cập đầy đủ thiết bị gym trong 1 ngày'),
    (N'Gói Thử Nghiệm 1 Tuần', N'Tiêu chuẩn', 150000.00, 7, N'Truy cập đầy đủ thiết bị gym trong 1 tuần'),
    (N'Gói Cơ Bản 1 Tháng', N'Tiêu chuẩn', 500000.00, 30, N'Truy cập không giới hạn thiết bị gym trong 1 tháng'),
    (N'Gói Cơ Bản 3 Tháng', N'Tiêu chuẩn', 1350000.00, 90, N'Gói tiết kiệm, truy cập thiết bị gym trong 3 tháng'),
    (N'Gói Cơ Bản 6 Tháng', N'Tiêu chuẩn', 2550000.00, 180, N'Gói trung hạn, truy cập thiết bị gym trong 6 tháng'),
    (N'Gói Cơ Bản 1 Năm', N'Tiêu chuẩn', 4800000.00, 365, N'Gói dài hạn, truy cập thiết bị gym cả năm, tiết kiệm nhất'),
    (N'Gói Huấn Luyện Cá Nhân 1 Tháng', N'Huấn luyện cá nhân', 2000000.00, 30, N'Tập với huấn luyện viên cá nhân, lịch tùy chỉnh'),
    (N'Gói Huấn Luyện Cá Nhân 3 Tháng', N'Huấn luyện cá nhân', 5400000.00, 90, N'Gói tiết kiệm, tập với huấn luyện viên cá nhân 3 tháng'),
    (N'Gói Nhóm 1 Tháng', N'Nhóm', 1200000.00, 30, N'Gói cho 2-3 người, truy cập thiết bị gym không giới hạn'),
    (N'Gói Nhóm 3 Tháng', N'Nhóm', 3240000.00, 90, N'Gói tiết kiệm cho 2-3 người, truy cập thiết bị gym'),
    (N'Gói Nhóm 1 Năm', N'Nhóm', 11520000.00, 365, N'Gói dài hạn cho 2-3 người, truy cập thiết bị gym'),
    (N'Gói Tăng Cơ 3 Tháng', N'Tiêu chuẩn', 1500000.00, 90, N'Gói tập trung tăng cơ, truy cập thiết bị gym và tư vấn');

-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
--                                           TRINH
INSERT INTO TrainingSchedule (trainerId, memberId, trainingDate, startTime, endTime, node)
VALUES 
(8, 8, '2025-06-15', '2025-06-15 07:00:00', '2025-06-15 09:00:00', N'Tập gym tăng cơ - Thứ 3-7: 7h-11h, 15h-19h'),
(8, 10, '2025-06-15', '2025-06-15 09:30:00', '2025-06-15 11:30:00', N'Tập gym tăng cơ - Thứ 3-7: 7h-11h, 15h-19h'),
(8, 6, '2025-06-15', '2025-06-15 15:00:00', '2025-06-15 17:00:00', N'Tập gym tăng cơ - Thứ 3-7: 7h-11h, 15h-19h');
-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
--                                           HẢI NÈ
INSERT INTO TrainingSchedule (trainerId, memberId, trainingDate, startTime, endTime, node)
VALUES 
(8, 18, '2025-06-15', '2025-06-15 07:00:00', '2025-06-15 09:00:00', N'Tập gym tăng cơ - Thứ 3-7: 7h-11h, 15h-19h'),
(8, 20, '2025-06-15', '2025-06-15 09:30:00', '2025-06-15 11:30:00', N'Tập gym tăng cơ - Thứ 3-7: 7h-11h, 15h-19h'),
(8, 16, '2025-06-15', '2025-06-15 15:00:00', '2025-06-15 17:00:00', N'Tập gym tăng cơ - Thứ 3-7: 7h-11h, 15h-19h');
-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


-- Thêm ràng buộc (Vì insert dữ liệu rồi nên không thêm trực tiếp được, phải cho null col Type sau đó thêm ràng buộc rồi thêm Type vào)
-- Cho tất cả type=null
UPDATE [GYM].[dbo].[Package]
SET [type] = null
WHERE [type] NOT IN (N'...');
go
-- ràng buộc
ALTER TABLE [GYM].[dbo].[Package]
ADD CONSTRAINT CHK_Package_Type 
CHECK ([type] IN (N'Tiêu chuẩn', N'Huấn luyện cá nhân', N'Nhóm', N'Huấn luyện nhóm'));
go
-- thêm lại type
UPDATE [GYM].[dbo].[Package]
SET [type] = 
    CASE 
        WHEN [name] IN (N'Gói 1 Ngày', N'Gói Thử Nghiệm 1 Tuần', N'Gói Cơ Bản 1 Tháng', N'Gói Cơ Bản 3 Tháng', N'Gói Cơ Bản 6 Tháng', N'Gói Cơ Bản 1 Năm', N'Gói Tăng Cơ 3 Tháng') THEN N'Tiêu chuẩn'
        WHEN [name] IN (N'Gói Huấn Luyện Cá Nhân 1 Tháng', N'Gói Huấn Luyện Cá Nhân 3 Tháng') THEN N'Huấn luyện cá nhân'
        WHEN [name] IN (N'Gói Nhóm 1 Tháng', N'Gói Nhóm 3 Tháng', N'Gói Nhóm 1 Năm') THEN N'Nhóm'
        ELSE N'Tiêu chuẩn' -- Giá trị mặc định cho các gói không xác định
    END
WHERE [type] IS NULL;
go
-- Thêm isActive (default true) vào package để ẩn những gói nào không còn áp dụng nữa.
ALTER TABLE [GYM].[dbo].[Package]
ADD isActive BIT NOT NULL DEFAULT 1;
go
-- Thêm image vào Trainer 
ALTER TABLE [GYM].[dbo].[Trainer]
ADD image NVARCHAR(255) NULL;
go
-- Thêm Note vs Description vào Payment
ALTER TABLE [GYM].[dbo].[Payment]
ADD note NVARCHAR(255) NULL,
    description NVARCHAR(255) NULL;

-- Đổi thuộc tính amount thành total trong Payment
EXEC sp_rename 
    '[GYM].[dbo].[Payment].amount', 
    'total', 
    'COLUMN';
go

-- Thêm isPaid trong Payment
-- Thêm dueDate trong Payment
ALTER TABLE [GYM].[dbo].[Payment]
ADD isPaid BIT NOT NULL DEFAULT 0,
    dueDate DATE NULL;





-- Xoá primary key
ALTER TABLE MemberPayment DROP CONSTRAINT PK_MEMBERPAYMENT;

-- Thay đổi cột cho phép null
ALTER TABLE MemberPayment ALTER COLUMN staffId INT NULL;

-- Tạo lại PK (ví dụ chỉ dùng MemberId và PaymentId)
ALTER TABLE MemberPayment ADD CONSTRAINT PK_MEMBERPAYMENT PRIMARY KEY (MemberId, PaymentId);

-- Cho phép null
ALTER TABLE MemberPayment
ADD CONSTRAINT FK_MemberPayment_Staff FOREIGN KEY (staffId)
REFERENCES Staff(StaffId);


-- Thêm liên kết giữa MemberPackage với Payment
ALTER TABLE MemberPakage
ADD paymentId INT NULL;

-- (Optional) Thêm khóa ngoại nếu muốn ràng buộc
ALTER TABLE MemberPakage
ADD CONSTRAINT FK_MemberPakage_Payment
FOREIGN KEY (paymentId) REFERENCES Payment(paymentId);

-- Thêm Gender vào Trainer
ALTER TABLE Trainer
ADD Gender BIT NULL;

UPDATE [GYM].[dbo].[Trainer]
SET [Gender] = CASE 
    WHEN trainerId IN (1, 3, 5, 6, 9) THEN 'true'
    ELSE 'false'
END


-- Thêm ảnh
UPDATE [GYM].[dbo].[Trainer]
SET [image] = 'images/trainer/van-loc.jpg'
WHERE [fullName] = N'Nguyễn Văn Lộc';

UPDATE [GYM].[dbo].[Trainer]
SET [image] = 'images/trainer/thi-linh.jpg'
WHERE [fullName] = N'Trần Thị Linh';

UPDATE [GYM].[dbo].[Trainer]
SET [image] = 'images/trainer/van-thien.jpg'
WHERE [fullName] = N'Phạm Văn Thiên';

UPDATE [GYM].[dbo].[Trainer]
SET [image] = 'images/trainer/thi-thao.jpg'
WHERE [fullName] = N'Lê Thị Thảo';

UPDATE [GYM].[dbo].[Trainer]
SET [image] = 'images/trainer/van-khoi.jpg'
WHERE [fullName] = N'Hồ Văn Khôi';

UPDATE [GYM].[dbo].[Trainer]
SET [image] = 'images/trainer/duc-tuan.jpg'
WHERE [fullName] = N'Vương Đức Tuấn';

UPDATE [GYM].[dbo].[Trainer]
SET [image] = 'images/trainer/thi-an.jpg'
WHERE [fullName] = N'Đặng Thị An';

UPDATE [GYM].[dbo].[Trainer]
SET [image] = 'images/trainer/thi-thuy.jpg'
WHERE [fullName] = N'Đỗ Thị Thủy';

UPDATE [GYM].[dbo].[Trainer]
SET [image] = 'images/trainer/hoang-phi.jpg'
WHERE [fullName] = N'Trương Hoàng Phi';

UPDATE [GYM].[dbo].[Trainer]
SET [image] = 'images/trainer/thi-phuong.jpg'
WHERE [fullName] = N'Bùi Thị Phương';
