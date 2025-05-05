CREATE TABLE tbl_court (
  id INT IDENTITY(1,1) PRIMARY KEY,
  user_id INT,
  court_name NVARCHAR(100),
  court_description NVARCHAR(255),
  district NVARCHAR(50),
  ward NVARCHAR(50),
  street NVARCHAR(50),
  images text,
  min_price FLOAT,
  max_price FLOAT,
  create_date DATETIME,
  update_date DATETIME,
  contact_person NVARCHAR(50),
  contact_phone VARCHAR(11),
  status INT
)
GO
CREATE TABLE tbl_child_court
(
  id INT IDENTITY(1,1) PRIMARY KEY,
  court_id INT,
  child_court_name nvarchar(100),
  child_court_description NVARCHAR(255),
  position nvarchar(100),
  rent_cost FLOAT
)
GO

CREATE TABLE tbl_user (
  id INT IDENTITY(1,1) PRIMARY KEY,
  username VARCHAR(50),
  password VARCHAR(100),
  fullname NVARCHAR(100),
  phone_number VARCHAR(11),
  address NVARCHAR(255),
  email VARCHAR(50),
  role INT,
  create_date DATETIME,
  last_login DATETIME,
  salt VARCHAR(30),
  token VARCHAR(MAX)
)
GO

CREATE TABLE tbl_booking (
  id INT IDENTITY(1,1) PRIMARY KEY,
  user_id INT,
  child_court_id INT,
  create_date DATETIME,
  start_time DATETIME,
  end_time DATETIME,
  status INT,
  price float
)
GO

CREATE TABLE tbl_review (
  id INT IDENTITY(1,1) PRIMARY KEY,
  user_id INT,
  court_id INT,
  content NVARCHAR(MAX),
  rating_star INT,
  create_date DATETIME,
  update_date DATETIME
)
GO

CREATE TABLE tbl_payment (
  id INT IDENTITY(1,1) PRIMARY KEY,
  user_id INT,
  booking_id INT,
  create_date DATETIME,
  price FLOAT,
  type INT
)
CREATE TABLE tbl_report (
  id INT IDENTITY(1,1) PRIMARY KEY,
  user_id INT,
  court_id INT,
  content NVARCHAR(MAX),
  create_date DATETIME,
  status INT
)
GO