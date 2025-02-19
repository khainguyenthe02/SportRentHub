CREATE TABLE tbl_court (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT,
    court_name NVARCHAR(100),
    court_description NVARCHAR(255),
    district NVARCHAR(50),
    ward NVARCHAR(50),
    street NVARCHAR(50),
    min_price FLOAT,
    max_price FLOAT,
    create_date DATETIME,
    update_date DATETIME,
    contact_person NVARCHAR(50),
    contact_phone VARCHAR(11),
    status INT
);

CREATE TABLE tbl_user (
    id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50),
    password VARCHAR(50),
    fullname VARCHAR(50),
    phone_number VARCHAR(11),
    address NVARCHAR(255),
    email VARCHAR(50),
    role INT,
    create_time DATETIME,
    last_login DATETIME
);

CREATE TABLE tbl_booking (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT,
    court_id INT,
    create_date DATETIME,
    booking_date DATETIME,
    start_time FLOAT,
    end_time FLOAT,
    status INT
);

CREATE TABLE tbl_review (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT,
    court_id INT,
    content TEXT,
    rating_star INT,
    create_date DATETIME,
    update_date DATETIME
);
