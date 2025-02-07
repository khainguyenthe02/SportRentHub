CREATE TABLE tbl_user (
    id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    password VARCHAR(50) NOT NULL,
    fullname NVARCHAR(50) NOT NULL,
    phone_number VARCHAR(11) NOT NULL,
    address NVARCHAR(255),
    email VARCHAR(50) UNIQUE,
    role INT NOT NULL,
    create_time DATETIME DEFAULT GETDATE(),
    last_login DATETIME NULL,
	salt varchar(20)
);

CREATE TABLE tbl_court (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    court_name NVARCHAR(100) NOT NULL,
    court_description NVARCHAR(255),
    district NVARCHAR(50),
    ward NVARCHAR(50),
    street NVARCHAR(50),
    price_per_hour FLOAT NOT NULL,
    create_date DATETIME DEFAULT GETDATE(),
    contact_person NVARCHAR(50),
    contact_phone VARCHAR(11),
    status INT NOT NULL
);

CREATE TABLE tbl_booking (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    court_id INT NOT NULL,
    booking_date DATETIME NOT NULL,
    time_frame INT NOT NULL,
    status INT NOT NULL
);

CREATE TABLE tbl_review (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    court_id INT NOT NULL,
    content TEXT,
    rating_star INT CHECK (rating_star BETWEEN 1 AND 5),
    create_date DATETIME DEFAULT GETDATE()
	);
