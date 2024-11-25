CREATE DATABASE PetStoreDB;
GO

USE PetStoreDB;
GO

-- ��������� ������� Employee
CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Position NVARCHAR(50)
);

-- ��������� ������� Owner
CREATE TABLE Owner (
    OwnerID INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Phone NVARCHAR(15)
);

-- ��������� ������� Product
CREATE TABLE Product (
    ProductID INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(50),
    Price DECIMAL(10, 2),
    Stock INT
);

-- ��������� ������� Pet
CREATE TABLE Pet (
    PetID INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(50),
    Species NVARCHAR(50),
    Breed NVARCHAR(50),
    Age INT
);

-- ��������� ������� Order (�� �������� ������� �� ������� Owner � Employee)
CREATE TABLE [Order] (
    OrderID INT PRIMARY KEY IDENTITY,
    OrderDate DATE,
    OwnerID INT,
    EmployeeID INT,
    FOREIGN KEY (OwnerID) REFERENCES Owner(OwnerID),
    FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID)
);

-- ��������� ������� OrderProduct (�� �������� ������� �� ������� Order � Product)
CREATE TABLE OrderProduct (
    OrderID INT,
    ProductID INT,
    Quantity INT,
    PRIMARY KEY (OrderID, ProductID),
    FOREIGN KEY (OrderID) REFERENCES [Order](OrderID),
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);

-- ���������� ������� ������ ��� ����������
-- ��������� ����� �� ������� Employee
INSERT INTO Employee (FirstName, LastName, Position) 
VALUES ('John', 'Doe', 'Manager'), 
       ('Jane', 'Smith', 'Cashier'),
       ('Robert', 'Brown', 'Sales');

-- ��������� ����� �� ������� Owner
INSERT INTO Owner (FirstName, LastName, Phone) 
VALUES ('Alice', 'Johnson', '1234567890'), 
       ('Bob', 'Williams', '0987654321');

-- ��������� ����� �� ������� Product
INSERT INTO Product (Name, Price, Stock) 
VALUES ('Dog Food', 25.99, 100),
       ('Cat Toy', 5.50, 200),
       ('Fish Tank', 100.00, 10);

-- ��������� ����� �� ������� Pet
INSERT INTO Pet (Name, Species, Breed, Age) 
VALUES ('Buddy', 'Dog', 'Labrador', 5),
       ('Whiskers', 'Cat', 'Persian', 3),
       ('Goldie', 'Fish', 'Goldfish', 1);

-- ��������� ����� �� ������� Order
INSERT INTO [Order] (OrderDate, OwnerID, EmployeeID)
VALUES ('2024-10-01', 1, 1),
       ('2024-10-02', 2, 2);

-- ��������� ����� �� ������� OrderProduct
INSERT INTO OrderProduct (OrderID, ProductID, Quantity) 
VALUES (1, 1, 2), 
       (1, 2, 3), 
       (2, 3, 1);
