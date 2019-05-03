CREATE TABLE `Institution`
(
  `InstitutionId` int PRIMARY KEY,
  `InstitutionName` varchar(255),
  `Website` varchar(255),
  `PhoneNumber` varchar(255),
  `AddressLine1` varchar(255),
  `AddressLine2` varchar(255)
);

CREATE TABLE `user`
(
  `UserId` int PRIMARY KEY,
  `FirstName` varchar(255),
  `LastName` varchar(255),
  `UserName` varchar(255),
  `PasswordHash` binary,
  `PasswordSalt` binary,
  `CreatedBy` int,
  `CreatedDate` datetime,
  `ModifiedBy` int,
  `ModfiedDate` datetime
);

CREATE TABLE `Role`
(
  `RoleId` int PRIMARY KEY,
  `RoleName` varchar(255),
  `RoleDescription` varchar(255)
);

CREATE TABLE `Department`
(
  `DepartmentId` int PRIMARY KEY,
  `DepartmentName` varchar(255),
  `DepartmentDescription` varchar(255)
);

CREATE TABLE `UserRole`
(
   `Id` int PRIMARY KEY,
  `UserId` int,
  `RoleId` int
);

CREATE TABLE `UserDepartment`
(
   `Id` int PRIMARY KEY,
  `UserId` int,
  `DepartmentId` int
);

CREATE TABLE `UserInstitution`
(
   `Id` int PRIMARY KEY,
  `UserId` int,
  `InstitutionId` int
);

CREATE TABLE `InstitutionDepartment`
(
   `Id` int PRIMARY KEY,
   `InstitutionId` int,
    `DepartmentId` int,
);

ALTER TABLE `UserRole` ADD FOREIGN KEY (`UserId`) REFERENCES `user` (`UserId`);

ALTER TABLE `UserRole` ADD FOREIGN KEY (`RoleId`) REFERENCES `Role` (`RoleId`);

ALTER TABLE `UserDepartment` ADD FOREIGN KEY (`UserId`) REFERENCES `user` (`UserId`);

ALTER TABLE `UserDepartment` ADD FOREIGN KEY (`DepartmentId`) REFERENCES `Department` (`DepartmentId`);

ALTER TABLE `UserInstitution` ADD FOREIGN KEY (`UserId`) REFERENCES `user` (`UserId`);

ALTER TABLE `UserInstitution` ADD FOREIGN KEY (`InstitutionId`) REFERENCES `Institution` (`InstitutionId`);

ALTER TABLE `InstitutionDepartment` ADD FOREIGN KEY (`UserId`) REFERENCES `user` (`UserId`);

ALTER TABLE `InstitutionDepartment` ADD FOREIGN KEY (`DepartmentId`) REFERENCES `Department` (`DepartmentId`);
