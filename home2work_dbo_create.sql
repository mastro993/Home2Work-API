create database home2work collate SQL_Latin1_General_CP1_CI_AS
go

create table company
(
	id int identity
		primary key,
	name nvarchar(50) not null,
	domain nvarchar(250),
	latitude decimal(12,9),
	longitude decimal(12,9),
	region nvarchar(100),
	district nvarchar(50),
	cap nchar(5),
	city nvarchar(50),
	address nvarchar(250),
	registration datetime constraint company_registration_default default getdate()
)
go

create table api_keys
(
	key varchar(64) not null
		primary key,
	enabled bit default 0,
	label text
)
go

create unique index api_key_key_uindex
	on api_keys (key)
go

create table company_status
(
	id int identity
		primary key,
	company_id int not null
		constraint company_status_company_id_fk
			references company,
	status text,
	date datetime default getdate(),
	hidden bit default 0
)
go

create table users
(
	id int identity
		primary key,
	email nvarchar(320) not null,
	password_hash char(64) not null,
	salt char(64) not null,
	name nvarchar(50),
	surname nvarchar(50),
	birthday date,
	company_id int,
	registration_date date default getdate() not null,
	home_region nvarchar(50),
	home_district nvarchar(50),
	home_cap nchar(5),
	home_city nvarchar(50),
	home_address nvarchar(250),
	home_latitude decimal(12,9),
	home_longitude decimal(12,9),
	job_region nvarchar(50),
	job_district nvarchar(50),
	job_cap nchar(5),
	job_city nvarchar(50),
	job_address nvarchar(250),
	job_latitude decimal(12,9),
	job_longitude decimal(12,9),
	job_start_time time,
	job_end_time time,
	firebase_token varchar(250)
)
go

create table scl_location
(
	id int identity
		primary key,
	user_id int not null
		constraint location_users_id_fk
			references users,
	time datetime not null,
	latitude decimal(12,9),
	longitude decimal(12,9),
	type tinyint
)
go

create unique index location_time_latitude_longitude_uindex
	on scl_location (time, latitude, longitude)
go

create table last_location
(
	id int identity
		primary key,
	user_id int not null
		constraint last_location_users_id_fk
			references users,
	latitude decimal(12,9),
	longitude decimal(12,9),
	time datetime not null
)
go

create table user_routine
(
	id int identity
		primary key,
	user_id int not null
		constraint routine_users_id_fk
			references users,
	start_lat decimal(12,9),
	start_lng decimal(12,9),
	start_time time,
	finish_lat decimal(12,9),
	finish_lng decimal(12,9),
	finish_time time,
	type int
)
go

create table fcm_token
(
	id int identity
		primary key,
	user_id int not null
		constraint fcm_token_users_id_fk
			references users,
	token varchar(250) not null
)
go

create table chat
(
	id int identity
		primary key,
	user_id1 int not null
		constraint chat_users_id_fk
			references users
				on update cascade on delete cascade,
	user_id2 int not null,
	date datetime default getdate() not null
)
go

exec sp_addextendedproperty 'MS_Description', 'Lista delle conversazioni tra', 'SCHEMA', 'dbo', 'TABLE', 'chat'
go

create table message
(
	id int identity
		primary key,
	sender_id int not null
		constraint message_users_id_fk
			references users,
	chat_id int not null
		constraint message_chat_id_fk
			references chat,
	text text,
	time datetime default getdate() not null,
	new bit default 0 not null
)
go

create table karma
(
	id int identity
		primary key,
	user_id int not null
		constraint karma_users_id_fk
			references users,
	amount int default 0 not null,
	time datetime default getdate() not null
)
go

create unique index user_exp_id_uindex
	on karma (id)
go

create table match
(
	match_id int identity
		primary key,
	user_id int not null
		constraint match_users_id_fk_2
			references users,
	host_id int not null
		constraint match_users_id_fk
			references users,
	home_score int default 0 not null,
	job_score int default 0 not null,
	time_score int default 0 not null,
	arrival_time time not null,
	departure_time time,
	new bit default 1,
	hidden bit default 0,
	distance int default 0 not null
)
go

create table share
(
	id int identity
		primary key,
	host_id int not null
		constraint share_users_id_fk
			references users,
	status int default 0 not null,
	time datetime default getdate() not null,
	start_latitude decimal(12,9) default 0 not null,
	start_longitude decimal(12,9) default 0 not null,
	end_latitude decimal(12,9),
	end_longitude decimal(12,9),
	distance int default 0 not null
)
go

create table session
(
	user_id int not null
		constraint session_users_id_fk
			references users,
	session_id nchar(64) not null
)
go

create unique index session_user_id_uindex
	on session (user_id)
go

create unique index session_session_id_uindex
	on session (session_id)
go

create table share_guest
(
	share_id int not null,
	user_id int not null
		constraint share_guest_users_id_fk
			references users,
	start_time datetime default getdate() not null,
	status int default 0 not null,
	distance int constraint ShareGuests_Distance_default default 0,
	start_latitude decimal(12,9) default 0 not null,
	start_longitude decimal(12,9) default 0 not null,
	end_latitude decimal(12,9),
	end_longitude decimal(12,9),
	constraint ShareGuests_ShareId_GuestId_pk
		primary key (share_id, user_id)
)
go

create table user_status
(
	id int identity
		primary key,
	user_id int not null
		constraint user_status_users_id_fk
			references users,
	status text,
	date datetime default getdate(),
	hidden bit default 0
)
go

CREATE VIEW company_statistics AS
  SELECT
    C.id,
    C.name                                                     AS name,
    company_employees.employees                                AS employees,
    (SELECT COALESCE(SUM(shares), 0)
     FROM user_total_shares
     WHERE user_total_shares.company_id = C.id)                AS total_shares,
    (SELECT CAST(COALESCE(SUM(shares), 0) AS FLOAT) / CAST(dbo.InlineMax(company_employees.employees, 1) AS FLOAT)
     FROM user_total_shares
     WHERE user_total_shares.company_id = C.id)                AS user_avg_shares,
    (SELECT COALESCE(SUM(month_shares), 0)
     FROM user_monthly_shares
     WHERE user_monthly_shares.company_id = C.id)              AS monthly_shares,
    (SELECT CAST(COALESCE(SUM(month_shares), 0) AS FLOAT) / CAST(dbo.InlineMax(company_employees.employees, 1) AS FLOAT)
     FROM user_monthly_shares
     WHERE user_monthly_shares.company_id = C.id)              AS user_avg_monthly_shares,
    (SELECT COALESCE(AVG(shares_avg), 0)
     FROM user_monthly_shares_avg
     WHERE user_monthly_shares_avg.company_id = C.id)          AS monthly_shares_avg,
    (SELECT COALESCE(SUM(shared_distance), 0)
     FROM user_total_shared_distance
     WHERE user_total_shared_distance.company_id = C.id)       AS total_shared_distance,
    (SELECT
       CAST(COALESCE(SUM(shared_distance), 0) AS FLOAT) / CAST(dbo.InlineMax(company_employees.employees, 1) AS FLOAT)
     FROM user_total_shared_distance
     WHERE user_total_shared_distance.company_id = C.id)       AS user_avg_shared_distance,
    (SELECT COALESCE(SUM(user_monthly_shared_distance.month_shared_distance), 0)
     FROM user_monthly_shared_distance
     WHERE user_monthly_shared_distance.company_id = C.id)     AS monthly_shared_distance,
    (SELECT CAST(COALESCE(SUM(user_monthly_shared_distance.month_shared_distance), 0) AS FLOAT) /
            CAST(dbo.InlineMax(company_employees.employees, 1) AS FLOAT)
     FROM user_monthly_shared_distance
     WHERE user_monthly_shared_distance.company_id = C.id)     AS user_avg_monthly_shared_distance,
    (SELECT COALESCE(AVG(month_shared_distance_avg), 0)
     FROM user_monthly_shared_distance_avg
     WHERE user_monthly_shared_distance_avg.company_id = C.id) AS monthly_shared_distance_avg
  FROM company C LEFT JOIN company_employees ON C.id = company_employees.Id
go

CREATE VIEW company_ranks AS
  SELECT
    id,
    name,
    RANK()
    OVER (
      ORDER BY total_shares DESC )                     AS total_shares_rank,
    RANK()
    OVER (
      ORDER BY user_avg_shares DESC )                  AS user_avg_shares_rank,
    RANK()
    OVER (
      ORDER BY monthly_shares DESC )                   AS monthly_shares_rank,
    RANK()
    OVER (
      ORDER BY user_avg_monthly_shares DESC )          AS user_avg_month_shares_rank,
    RANK()
    OVER (
      ORDER BY monthly_shares_avg DESC )               AS monthly_shares_avg_rank,
    RANK()
    OVER (
      ORDER BY total_shared_distance DESC )            AS total_shared_distance_rank,
    RANK()
    OVER (
      ORDER BY user_avg_shared_distance DESC )         AS user_avg_shared_distance_rank,
    RANK()
    OVER (
      ORDER BY monthly_shared_distance DESC )          AS monthly_shared_distance_rank,
    RANK()
    OVER (
      ORDER BY user_avg_monthly_shared_distance DESC ) AS user_avg_monthly_shared_distance_rank,
    RANK()
    OVER (
      ORDER BY monthly_shared_distance_avg DESC )      AS monthly_shared_distance_avg_rank
  FROM company_statistics
go

CREATE VIEW user_monthly_shares AS SELECT
  Id as user_id,
  Name as name,
  Surname as surname,
  company_id as company_id,
  (SELECT COUNT(DISTINCT s.id)
   FROM share S
     LEFT JOIN share_guest G ON S.Id = G.share_id
   WHERE (host_id = u.id OR G.user_id = u.id) AND MONTH(time) = MONTH(getdate()) AND S.status = 1) AS month_shares
FROM users u
go

CREATE VIEW user_monthly_shares_avg AS
  SELECT
    id                                                                  AS user_id,
    name                                                                AS name,
    surname                                                             AS surname,
    company_id                                                       AS company_id,
    (SELECT CAST(COUNT(DISTINCT S.id) AS FLOAT) /
            CAST(dbo.InlineMin(DATEDIFF(MONTH, registration_date, getdate()), 6) AS FLOAT)
     FROM share S
       LEFT JOIN share_guest G ON S.Id = G.user_id
     WHERE (host_id = u.id OR G.user_id = u.id) AND DATEDIFF(month, time, GETDATE()) <=
                                                    6 AND s.status = 1) AS shares_avg
  FROM users u
go

CREATE VIEW user_total_shared_distance AS
  SELECT
    id                                                                                                    AS user_id,
    name                                                                                                  AS name,
    surname                                                                                               AS surname,
    company_id                                                                                             AS company_id,
    (SELECT COALESCE(SUM(G.distance), 0)
   FROM share S
     LEFT JOIN share_guest G ON S.Id = G.share_id
   WHERE host_id = users.id OR G.user_id = users.id) AS shared_distance
  FROM users
go

CREATE VIEW user_monthly_shared_distance AS
  SELECT
    Id                                                                          AS user_id,
    Name                                                                        AS name,
    Surname                                                                     AS surname,
    company_id                                                                  AS company_id,
    (SELECT COALESCE(SUM(G.distance), 0)
     FROM share S
       LEFT JOIN share_guest G ON S.Id = G.share_id
     WHERE (host_id = users.id OR G.user_id = users.id) AND MONTH(Time) = MONTH(getdate())) AS month_shared_distance
  FROM users
go

CREATE VIEW user_total_shares AS
  SELECT
    id                                                              AS user_id,
    name                                                            AS name,
    surname                                                         AS surname,
    company_id                                                      AS company_id,
    (SELECT COUNT(DISTINCT S.id)
     FROM share S
       LEFT JOIN share_guest G ON S.id = G.share_id
     WHERE (S.host_id = U.id OR G.user_id = U.id) AND S.status = 1) AS shares,
    (SELECT COUNT(DISTINCT S.id)
     FROM share S
       LEFT JOIN share_guest G ON S.id = G.share_id
     WHERE S.host_id = U.id AND S.status = 1)                       AS host_shares,
    (SELECT COUNT(DISTINCT S.id)
     FROM share S
       LEFT JOIN share_guest G ON S.Id = G.share_id
     WHERE G.user_id = U.id AND S.Status = 1)                       AS guest_shares
  FROM users U
go

CREATE VIEW user_monthly_shared_distance_avg AS
 SELECT
  Id                                                 AS user_id,
  Name                                               AS name,
  Surname                                            AS surname,
  company_id                                      AS company_id,
  (SELECT COALESCE(CAST(SUM(G.distance) AS FLOAT) / CAST(dbo.InlineMin(DATEDIFF(month, registration_date, getdate()) + 1, 6) AS FLOAT), 0)
   FROM share S
     LEFT JOIN share_guest G ON S.Id = G.share_id
   WHERE (host_id = users.id OR G.user_id = users.id) AND DATEDIFF(month, time, getdate()) + 1 <=
                                                  6) AS month_shared_distance_avg
FROM users
go

CREATE VIEW user_statistics AS
  SELECT
    u.id                                                    AS user_id,
    u.name                                                  AS name,
    surname                                                 AS surname,
    company_id                                              AS company_id,
    c.name                                                  AS company_name,
    (SELECT shares
     FROM user_total_shares
     WHERE user_total_shares.user_id = U.id)                as total_shares,
    (SELECT guest_shares
     FROM user_total_shares
     WHERE user_total_shares.user_id = U.id)                as guest_shares,
    (SELECT host_shares
     FROM user_total_shares
     WHERE user_total_shares.user_id = U.id)                as host_shares,
    (SELECT month_shares
     FROM user_monthly_shares
     WHERE user_monthly_shares.user_id = U.id)              as monthly_shares,
    (SELECT weekly_shares
     FROM user_weekly_shares
     WHERE user_weekly_shares.user_id = U.id)               as weekly_shares,
    (SELECT shares_avg
     FROM user_monthly_shares_avg
     WHERE user_monthly_shares_avg.user_id = U.id)          as monthly_shares_avg,
    (SELECT COALESCE(top_shares, 0)
     FROM user_monthly_shares_record
     WHERE user_monthly_shares_record.user_id = U.id)       as monthly_shares_record,
    (SELECT COALESCE(longest_share, 0)
     FROM user_longest_share
     WHERE user_longest_share.user_id = U.id)               as longest_share,
    (SELECT shared_distance
     FROM user_total_shared_distance
     WHERE user_total_shared_distance.user_id = U.id)       as total_shared_distance,
    (SELECT month_shared_distance
     FROM user_monthly_shared_distance
     WHERE user_monthly_shared_distance.user_id = U.id)     as monthly_shared_distance,
    (SELECT weekly_shared_distance
     FROM user_weekly_shared_distance
     WHERE user_weekly_shared_distance.user_id = U.id)      as weekly_shared_distance,
    (SELECT month_shared_distance_avg
     FROM user_monthly_shared_distance_avg
     WHERE user_monthly_shared_distance_avg.user_id = U.id) as monthly_shared_distance_avg
  FROM users U LEFT JOIN company c ON c.id = U.company_id
go

CREATE VIEW company_employees AS
  SELECT
    C.id,
    C.name,
    (SELECT COUNT(*)
     FROM users U
     WHERE U.company_id = C.Id) AS employees
  FROM company C
go

CREATE VIEW chat_info AS
  SELECT
    *,
    (SELECT TOP 1 m.id
     FROM message m
     WHERE m.chat_id = c.id
     ORDER BY time DESC)                                           AS last_message_id,
    (SELECT COUNT(*) FROM message WHERE chat_id = c.id) AS message_count
  FROM chat c
go

CREATE VIEW user_monthly_shares_record AS
  SELECT
    u.id as user_id,
    COALESCE((SELECT TOP 1 COUNT(*)
      FROM users uu
        LEFT JOIN share s ON uu.id = s.host_id
        LEFT JOIN share_guest g ON s.id = g.share_id
      WHERE (uu.id = u.id OR g.user_id = u.id) AND s.status = 1
      GROUP BY MONTH(s.time)
      ORDER BY Count(*) DESC), 0)    AS top_shares
  FROM users u
go

CREATE VIEW user_longest_share AS
  SELECT
    u.id                       as user_id,
    COALESCE((SELECT TOP 1 g.distance
     FROM share s
       LEFT JOIN share_guest g ON s.id = g.share_id
     WHERE s.host_id = u.id OR g.user_id = u.id
     ORDER BY g.distance DESC), 0) as longest_share
  from users u
go

CREATE VIEW user_weekly_shared_distance AS
  SELECT
    Id            AS user_id,
    Name          AS name,
    Surname       AS surname,
    u.company_id AS company_id,
    (SELECT COALESCE(SUM(G.distance), 0)
     FROM share S
       LEFT JOIN share_guest G ON S.Id = G.share_id
     WHERE (host_id = U.Id OR G.user_id = U.Id)
           AND year(time) = YEAR(getdate())
           AND DATEPART(ww, time) = datepart(ww, getdate() - 1)
    )             AS weekly_shared_distance
  FROM users U
go

CREATE VIEW user_weekly_shares AS
  SELECT
    Id            as user_id,
    Name          as name,
    Surname       as surname,
    u.company_id as company_id,
    (
      SELECT COUNT(DISTINCT s.id)
      FROM share S
        LEFT JOIN share_guest G ON S.Id = G.share_id
      WHERE (host_id = U.Id OR G.user_id = U.Id)
            AND year(time) = YEAR(getdate())
            AND DATEPART(ww, time) = datepart(ww, getdate() - 1)
            AND S.status = 1
    )             AS weekly_shares
  FROM users U
go

CREATE VIEW user_weekly_shares_record AS
  SELECT
    u.id           as user_id,
    COALESCE((
               SELECT TOP 1 COUNT(*)
               FROM users uu
                 LEFT JOIN share s ON uu.id = s.host_id
                 LEFT JOIN share_guest g ON s.id = g.share_id
               WHERE (uu.id = u.id OR g.user_id = u.id)
                     AND s.status = 1
               GROUP BY DATEPART(ww, s.time), YEAR(s.time)
               ORDER BY Count(*) DESC
             ), 0) AS top_shares
  FROM users u
go

CREATE VIEW leaderboards_user_global AS
  SELECT
    user_id,
    name,
    surname,
    company_id,
    company_name,
    total_shares,
    RANK()
    OVER (
      ORDER BY total_shares DESC )            AS shares_alltime_rank,
    total_shared_distance,
    RANK()
    OVER (
      ORDER BY total_shared_distance DESC )   AS shared_distance_alltime_rank,
    weekly_shares,
    RANK()
    OVER (
      ORDER BY weekly_shares DESC )           AS shares_weekly_rank,
    weekly_shared_distance,
    RANK()
    OVER (
      ORDER BY weekly_shared_distance DESC )  AS shared_distance_weekly_rank,
    monthly_shares,
    RANK()
    OVER (
      ORDER BY monthly_shares DESC )          AS shares_monthly_rank,
    monthly_shared_distance,
    RANK()
    OVER (
      ORDER BY monthly_shared_distance DESC ) AS shared_distance_monthly_rank
  FROM user_statistics
go

CREATE VIEW leaderboards_user_company AS
  SELECT
    user_id,
    name,
    surname,
    company_id,
    company_name,
    total_shares,
    RANK()
    OVER ( PARTITION BY company_id
      ORDER BY total_shares DESC )            AS shares_alltime_rank,
    total_shared_distance,
    RANK()
    OVER ( PARTITION BY company_id
      ORDER BY total_shared_distance DESC )   AS shared_distance_alltime_rank,
    weekly_shares,
    RANK()
    OVER ( PARTITION BY company_id
      ORDER BY weekly_shares DESC )           AS shares_weekly_rank,
    weekly_shared_distance,
    RANK()
    OVER ( PARTITION BY company_id
      ORDER BY weekly_shared_distance DESC )  AS shared_distance_weekly_rank,
    monthly_shares,
    RANK()
    OVER ( PARTITION BY company_id
      ORDER BY monthly_shares DESC )          AS shares_monthly_rank,
    monthly_shared_distance,
    RANK()
    OVER ( PARTITION BY company_id
      ORDER BY monthly_shared_distance DESC ) AS shared_distance_monthly_rank
  FROM user_statistics
go

CREATE VIEW leaderboards_company AS

  SELECT
    id,
    name,
    RANK()
    OVER (
      ORDER BY total_shares DESC )            AS shares_alltime_rank,
    RANK()
    OVER (
      ORDER BY total_shared_distance DESC )   AS shared_distance_alltime_rank,
    RANK()
    OVER (
      ORDER BY monthly_shares DESC )          AS shares_monthly_rank,
    RANK()
    OVER (
      ORDER BY monthly_shared_distance DESC ) AS shared_distance_monthly_rank,
    RANK()
    OVER (
      ORDER BY total_shares DESC )            AS shares_weekly_rank,
    RANK()
    OVER (
      ORDER BY total_shared_distance DESC )   AS shared_distance_weekly_rank
  FROM company_statistics
go

CREATE PROCEDURE update_user_status @uId INT, @status TEXT AS
  BEGIN
    INSERT into user_status (user_id, status) VALUES (@uId, @status)
  END
go

CREATE PROCEDURE insert_message_to_chat
    @cId   INT,
    @uId   INT,
    @mText TEXT
AS
  BEGIN
    INSERT INTO message (chat_id, sender_id, text, new)
    OUTPUT INSERTED.ID
    VALUES (@cId, @uId, @mText, 1)
  END
go

CREATE PROCEDURE get_chat_message
    @mId INT
AS
  BEGIN
    SELECT *
    FROM message
    WHERE id = @mId
  END
go

CREATE PROCEDURE insert_new_chat
    @uId INT,
    @rId INT
AS
  BEGIN
    INSERT INTO chat (user_id1, user_id2)
    OUTPUT INSERTED.id
    VALUES (@uId, @rId)
  END
go

CREATE PROCEDURE get_user_chat
    @uId INT,
    @cId INT
AS
  BEGIN
    SELECT
      c.id,
      (
        SELECT CASE
               WHEN c.user1 = @uId
                 THEN c.user2
               WHEN c.user2 = @uId
                 THEN c.user1
               END
      )                                                              AS userId,
      c.time,
      COALESCE(c.last_message_id, 0)                                 AS last_message_id,
      c.message_count,
      (SELECT COUNT(*)
       FROM message m
       WHERE m.chat_id = c.id AND m.new = 1 AND m.sender_id != @uId) AS unread_count,
      COALESCE(m.sender_id, 0)                                       AS last_message_sender_id,
      COALESCE(m.text, '')                                           AS last_message_text,
      COALESCE(m.time, getdate())                                    AS last_message_time,
      COALESCE(m.new, cast(0 AS BIT))                                AS last_message_new
    FROM chat_info c
      LEFT JOIN message m ON c.last_message_id = m.id
    WHERE c.id = @cId AND (c.user1 = @uId OR c.user2 = @uId)
  END
go

CREATE PROCEDURE get_users_chat
    @uId INT,
    @rId INT
AS
  BEGIN
    SELECT
      c.id,
      (
        SELECT CASE
               WHEN c.user1 = @uId
                 THEN c.user2
               WHEN c.user2 = @uId
                 THEN c.user1
               END
      )                                                              AS userId,
      c.time,
      COALESCE(c.last_message_id, 0)                                 AS last_message_id,
      c.message_count,
      (SELECT COUNT(*)
       FROM message m
       WHERE m.chat_id = c.id AND m.new = 1 AND m.sender_id != @uId) AS unread_count,
      COALESCE(m.sender_id, 0)                                       AS last_message_sender_id,
      COALESCE(m.text, '')                                           AS last_message_text,
      COALESCE(m.time, getdate())                                    AS last_message_time,
      COALESCE(m.new, cast(0 AS BIT))                                AS last_message_new
    FROM chat_info c
      LEFT JOIN message m ON c.last_message_id = m.id
    WHERE (c.user1 = @uId AND c.user2 = @rId)
          OR (c.user1 = @rId AND c.user2 = @uId)
    ORDER BY last_message_new DESC, last_message_time DESC
  END
go

CREATE PROCEDURE get_user_chat_list
    @uId INT
AS
  BEGIN
    SELECT
      c.id,
      (
        SELECT CASE
               WHEN c.user1 = @uId
                 THEN c.user2
               WHEN c.user2 = @uId
                 THEN c.user1
               END
      )                                                              AS userId,
      c.time,
      c.last_message_id,
      c.message_count,
      (SELECT COUNT(*)
       FROM message m
       WHERE m.chat_id = c.id AND m.new = 1 AND m.sender_id != @uId) AS unread_count,
      m.sender_id                                                    AS last_message_sender_id,
      m.text                                                         AS last_message_text,
      m.time                                                         AS last_message_time,
      m.new                                                          AS last_message_new
    FROM chat_info c
      LEFT JOIN message m ON c.last_message_id = m.id
    WHERE (c.user1 = @uId OR c.user2 = @uId) AND message_count > 0
    ORDER BY last_message_new DESC, last_message_time DESC
  END
go

CREATE PROCEDURE set_messages_as_read @cId INT, @uId INT AS
  BEGIN
    UPDATE message
    SET new = 0
    WHERE chat_id = @cId AND sender_id != @uId
  END
go

CREATE PROCEDURE get_chat_messages @cId INT, @uId INT AS
  BEGIN
    SELECT m2.*
    FROM chat c
      LEFT JOIN message m2 ON c.id = m2.chat_id
    WHERE chat_id = @cId AND (c.user_id1 = @uId OR c.user_id2 = @uId)
  END
go

CREATE PROCEDURE set_user_fcm_token @uId INT, @token VARCHAR(MAX) AS
  BEGIN
    INSERT INTO fcm_token (user_id, token)
    VALUES (@uId, @token)
  END
go

CREATE PROCEDURE insert_user_scl_location @userId INT, @latitude DECIMAL(12, 9), @longitude DECIMAL(12, 9), @type INT,
                                     @date   DATETIME AS
  BEGIN

    IF NOT EXISTS(SELECT *
                  FROM scl_location
                  WHERE user_id = @userId AND latitude = @latitude AND longitude = @longitude and time = @date)
      BEGIN
        INSERT INTO scl_location (user_id, latitude, longitude, time, type)
        OUTPUT Inserted.Id
        VALUES (@userId, @latitude, @longitude, @date, @type)
      END
  END
go

CREATE PROCEDURE update_user_fcm_token
    @uId INT,
    @token VARCHAR(MAX)
AS
  BEGIN
    UPDATE fcm_token
    SET token = @token
    WHERE user_id = @uId
  END
go

CREATE PROCEDURE insert_user_last_location @userId INT, @latitude DECIMAL(12, 9), @longitude DECIMAL(12, 9),
                                           @date   DATETIME AS
  BEGIN

    IF NOT EXISTS(SELECT *
                  FROM last_location
                  WHERE user_id = @userId)
      BEGIN
        INSERT INTO last_location (user_id, latitude, longitude, time)
        VALUES (@userId, @latitude, @longitude, @date)
      END
    ELSE
      UPDATE last_location
      set latitude = @latitude,
        longitude  = @longitude,
        time       = @date
      WHERE user_id = @userId
  END
go

CREATE PROCEDURE insert_user_match
    @userId        INT,
    @hostId        INT,
    @homeScore     INT,
    @jobScore      INT,
    @timeScore     INT,
    @arrivalTime   TIME,
    @departureTime TIME,
    @distance      INT
AS
  BEGIN
    INSERT INTO match (user_id, host_id, home_score, job_score, time_score, arrival_time, departure_time, distance)
    VALUES (@UserId, @HostId, @HomeScore, @JobScore, @TimeScore, @ArrivalTime, @DepartureTime, @Distance)
  END
go

CREATE PROCEDURE get_user_matches @uId INT, @limit BIGINT = 100, @page INT = 1 AS
  BEGIN
    SELECT *
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY (home_score + job_score + time_score) / 3 DESC ) AS number,
            *
          FROM match m
          WHERE user_id = @uId AND hidden = 0
         ) AS RowConstrainedResult
    WHERE number >= ((@page - 1) * @limit + 1)
          AND number < (@page * @limit + 1)
    ORDER BY number
  END
go

CREATE PROCEDURE get_match @mId INT AS
  BEGIN
    SELECT * FROM match WHERE match_id = @mId
  END
go

CREATE PROCEDURE edit_match @mId INT, @isNew BIT, @isHidden BIT AS
  BEGIN
    UPDATE match
    SET
      new    = @isNew,
      hidden = @isHidden
    WHERE match_id = @mId
  END
go

CREATE PROCEDURE hide_user_status @userId INT AS
  BEGIN
    UPDATE users_status
    SET hidden = 1
    WHERE id IN (SELECT TOP 1 id
                 FROM users_status
                 WHERE user_id = @userId
                 ORDER BY date DESC)
  END
go

CREATE PROCEDURE get_user_shares @uId INT, @limit BIGINT = 100, @page INT = 1 AS

  BEGIN
    SELECT *
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY s.time DESC ) AS number,
            *
          FROM share s
          WHERE (s.host_id = @uId AND s.status = 1)
                OR (s.id IN
                    (SELECT g.share_id
                     FROM share_guest g
                     WHERE g.user_id = @uId AND g.status = 1)
                    AND s.status = 1)
         ) AS RowConstrainedResult
    WHERE number >= ((@page - 1) * @limit + 1)
          AND number < (@page * @limit + 1)
    ORDER BY number
  END
go

CREATE PROCEDURE get_share @sId INT AS
  BEGIN
    SELECT * FROM share WHERE id = @sId
  END
go

CREATE PROCEDURE set_share_status @sId INT, @status INT AS
  BEGIN
    UPDATE share
    SET
      status = @status
    WHERE id = @sId
  END
go

CREATE PROCEDURE get_share_guest @sId INT, @gId INT AS
  BEGIN
    SELECT *
    FROM share_guest
    WHERE share_id = @sId AND user_id = @gId
  END
go

create or replace function FullMonthsSeparation(??? int, [@DateA] datetime, [@DateB] datetime) as -- missing source code
go

CREATE PROCEDURE insert_user_level_event @userId INT, @newLevel INT AS
  BEGIN
    INSERT INTO user_level_event (user_id, new_level) VALUES (@userId, @newLevel)
  END
go

CREATE PROCEDURE insert_share_guest
    @sId INT, 
    @gId INT, 
    @startLat DECIMAL(12, 9), 
    @startLng DECIMAL(12, 9) AS
  BEGIN
    INSERT INTO share_guest (share_id, user_id, start_latitude, start_longitude)
    VALUES (@sId, @gId, @startLat, @startLng)
  END
go

CREATE PROCEDURE set_share_guest_status @sId INT, @gId INT, @status INT AS
  BEGIN
    UPDATE share_guest
    SET
      status = @status
    WHERE share_id = @sId AND user_id = @gId
  END
go

CREATE PROCEDURE get_share_guests @sId INT AS
  BEGIN
    SELECT *
    FROM share_guest
    WHERE share_id = @sId
  END
go

create or replace function InlineMax(??? int, [@val1] int, [@val2] int) as -- missing source code
go

CREATE PROCEDURE user_login @email VARCHAR(255), @password_hash VARCHAR(255) AS
  BEGIN
    SELECT *
    FROM users
    WHERE email = @email AND password_hash = @password_hash
  END
go

CREATE PROCEDURE get_user_salt @email VARCHAR(255) AS
  BEGIN
    SELECT salt
    FROM users
    WHERE email = @email
  END
go

create or replace function InlineMin(??? int, [@val1] int, [@val2] int) as -- missing source code
go

CREATE PROCEDURE get_user @uId INT AS
  BEGIN
    SELECT *
    FROM users
    WHERE id = @uId
  END
go

CREATE PROCEDURE update_user_firebase_token @userId INT, @token varchar(250) AS
  BEGIN
    UPDATE users
    set firebase_token = @token
    WHERE id = @userId
  END
go

CREATE PROCEDURE set_session_token @uId INT, @token VARCHAR(64) AS
  BEGIN
    IF EXISTS(SELECT NULL
              FROM session
              WHERE user_id = @uId)
      UPDATE session
      SET session_id = @token
      WHERE user_id = @uId
    ELSE
      INSERT INTO session (user_id, session_id) values (@uId, @token)
  END
go

CREATE PROCEDURE get_user_firebase_token @userId INT AS
  BEGIN
      SELECT firebase_token FROM users WHERE id = @userId
  END
go

CREATE PROCEDURE get_user_from_session @token VARCHAR(64) AS
  BEGIN
    SELECT *
    FROM session
      LEFT JOIN users ON users.id = session.user_id
    WHERE session_id = @token
  END
go

CREATE PROCEDURE get_all_company_locations @companyId INT AS
  BEGIN
    SELECT s.*
    from scl_location s
      inner join users u on s.user_id = u.id
      inner join company c on u.company_id = c.id
    WHERE c.id = @companyId
  END
go

CREATE PROCEDURE add_user_karma @userId INT, @amount INT AS
  BEGIN
    INSERT INTO karma (user_id, amount)
    values (@userId, @amount)
  END
go

CREATE PROCEDURE get_all_user_locations @userId INT AS
  BEGIN
    SELECT *
    from scl_location
    WHERE user_id = @userId
  END
go

CREATE PROCEDURE get_user_statistics @uId INT AS
  BEGIN
    SELECT *
    FROM user_statistics US
    WHERE US.user_id = @uId
  END
go

CREATE PROCEDURE get_user_monthly_activity @uId INT AS
  BEGIN
    SELECT
      YEAR(s.time)     as year,
      MONTH(s.time)    as month,
      COUNT(*)         as shares,
      SUM(sg.distance) as distance
    FROM share s LEFT JOIN share_guest sg ON s.id = sg.share_id
    WHERE ((s.host_id = @uId AND s.status = 1) OR (sg.user_id = @uId AND sg.status = 1))
          AND s.time >= DATEADD(month, -12, DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0))
          AND s.time <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)
    GROUP BY YEAR(s.time), MONTH(s.time)
    ORDER BY YEAR(s.time) DESC, MONTH(s.time) DESC
  END
go

CREATE PROCEDURE get_user_locations_in_range
    @userId    INT,
    @latitude  DECIMAL(12, 9),
    @longitude DECIMAL(12, 9),
    @range     INT
AS
  BEGIN
    SELECT *
    FROM scl_location l
    WHERE (
            6371000 * acos(
                cos(radians(@latitude)) * cos(radians(latitude)) * cos(radians(longitude) - radians(@longitude))
                + sin(radians(@latitude)) * sin(radians(latitude))
            ) <= @range
          )
          AND l.user_id = @userId
  END
go

CREATE PROCEDURE get_user_locations_from_date
    @userId INT,
    @date   DATETIME
AS
  BEGIN
    SELECT * FROM scl_location l WHERE (l.time > @date)
                                   AND l.user_id = @userId
  END
go

CREATE PROCEDURE get_user_karma @userId INT AS
  BEGIN
    SELECT
      (SELECT COALESCE(SUM(amount), 0)
       FROM karma
       WHERe user_id = users.id)                                    AS total_karma,
      (SELECT COALESCE(SUM(amount), 0)
       FROM karma
       WHERe user_id = users.id AND MONTH(time) = MONTH(getdate())) AS month_karma
    FROM users
    WHERE id = @userId
  END
go

CREATE PROCEDURE complete_share @shareId  INT, @guestId INT, @latitude DECIMAL(12, 9), @longitude DECIMAL(12, 9),
                               @distance INT AS
  BEGIN
    UPDATE share_guest
    SET
      end_latitude  = @latitude,
      end_longitude = @longitude,
      status        = 1,
      distance      = @distance
    WHERE share_id = @shareId AND user_id = @guestId
  END
go

CREATE PROCEDURE finish_share @shareId INT,@latitude DECIMAL(12, 9), @longitude DECIMAL(12, 9), @distance INT AS
  BEGIN
    UPDATE share
    SET end_latitude = @latitude, end_longitude = @longitude, status = 1, distance=@distance
    WHERE id = @shareId
  END
go

CREATE PROCEDURE cancel_share @shareId INT AS
  BEGIN
    UPDATE share
      SET status=2
    WHERE id=@shareId
  END
go

CREATE PROCEDURE create_share @hostId INT, @latitude DECIMAL(12, 9), @longitude DECIMAL(12, 9) AS
  BEGIN
    INSERT INTO share (host_id, start_latitude, start_longitude)
    OUTPUT Inserted.Id
    VALUES (@hostId, @latitude, @longitude)
  END
go

CREATE PROCEDURE leave_share @shareId INT, @guestId INT AS
  BEGIN
    UPDATE share_guest
    SET
      status = 2
    WHERE share_id = @shareId AND user_id = @guestId
  END
go

CREATE PROCEDURE get_user_current_share @userId INT AS
  BEGIN
    SELECT s.*
    FROM share s LEFT JOIN share_guest g on s.id = g.share_id
    WHERE s.status = 0 AND (s.host_id = @userId OR (g.user_id = @userId AND g.status = 0))
  END
go

CREATE PROCEDURE get_users_leaderboard_global_alltime_shares @limit BIGINT = 100, @page INT = 1 AS
  BEGIN
    SELECT
      position,
      user_id,
      name,
      surname,
      company_id,
      company_name,
      total_shares as value
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY shares_alltime_rank ) AS position,
            *
          FROM leaderboards_user_global
         ) AS RowConstrainedResult
    WHERE position >= ((@page - 1) * @limit + 1)
          AND position < (@page * @limit + 1)
    ORDER BY position
  END
go

CREATE PROCEDURE get_users_leaderboard_global_alltime_distance @limit BIGINT = 100, @page INT = 1 AS
  BEGIN
    SELECT
      position,
      user_id,
      name,
      surname,
      company_id,
      company_name,
      total_shared_distance as value
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY shared_distance_alltime_rank ) AS position,
            *
          FROM leaderboards_user_global
         ) AS RowConstrainedResult
    WHERE position >= ((@page - 1) * @limit + 1)
          AND position < (@page * @limit + 1)
    ORDER BY position
  END
go

CREATE PROCEDURE get_users_leaderboard_global_monthly_distance @limit BIGINT = 100, @page INT = 1 AS
  BEGIN
    SELECT
      position,
      user_id,
      name,
      surname,
      company_id,
      company_name,
      monthly_shared_distance as value
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY shared_distance_monthly_rank ) AS position,
            *
          FROM leaderboards_user_global
         ) AS RowConstrainedResult
    WHERE position >= ((@page - 1) * @limit + 1)
          AND position < (@page * @limit + 1)
    ORDER BY position
  END
go

CREATE PROCEDURE get_users_leaderboard_global_monthly_shares @limit BIGINT = 100, @page INT = 1 AS
  BEGIN
    SELECT
      position,
      user_id,
      name,
      surname,
      company_id,
      company_name,
      monthly_shares as value
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY shares_monthly_rank ) AS position,
            *
          FROM leaderboards_user_global
         ) AS RowConstrainedResult
    WHERE position >= ((@page - 1) * @limit + 1)
          AND position < (@page * @limit + 1)
    ORDER BY position
  END
go

CREATE PROCEDURE get_users_leaderboard_global_weekly_shares @limit BIGINT = 100, @page INT = 1 AS
  BEGIN
    SELECT
      position,
      user_id,
      name,
      surname,
      company_id,
      company_name,
      weekly_shares as value
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY shared_distance_weekly_rank ) AS position,
            *
          FROM leaderboards_user_global
         ) AS RowConstrainedResult
    WHERE position >= ((@page - 1) * @limit + 1)
          AND position < (@page * @limit + 1)
    ORDER BY position
  END
go

CREATE PROCEDURE get_users_leaderboard_global_weekly_distance @limit BIGINT = 100, @page INT = 1 AS
  BEGIN
    SELECT
      position,
      user_id,
      name,
      surname,
      company_id,
      company_name,
      weekly_shared_distance as value
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY shared_distance_weekly_rank ) AS position,
            *
          FROM leaderboards_user_global
         ) AS RowConstrainedResult
    WHERE position >= ((@page - 1) * @limit + 1)
          AND position < (@page * @limit + 1)
    ORDER BY position
  END
go

CREATE PROCEDURE get_users_leaderboard_company_weekly_distance @companyId INT, @limit BIGINT = 100, @page INT = 1 AS
  BEGIN
    SELECT
      position,
      user_id,
      name,
      surname,
      company_id,
      company_name,
      weekly_shared_distance as value
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY shared_distance_weekly_rank ) AS position,
            *
          FROM leaderboards_user_company
      WHERE company_id = @companyId
         ) AS RowConstrainedResult
    WHERE position >= ((@page - 1) * @limit + 1)
          AND position < (@page * @limit + 1)
    ORDER BY position
  END
go

CREATE PROCEDURE get_users_leaderboard_company_monthly_distance @companyId INT, @limit BIGINT = 100, @page INT = 1 AS
  BEGIN
    SELECT
      position,
      user_id,
      name,
      surname,
      company_id,
      company_name,
      monthly_shared_distance as value
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY shared_distance_monthly_rank ) AS position,
            *
          FROM leaderboards_user_company
          where company_id = @companyId
         ) AS RowConstrainedResult
    WHERE position >= ((@page - 1) * @limit + 1)
          AND position < (@page * @limit + 1)
    ORDER BY position
  END
go

CREATE PROCEDURE get_users_leaderboard_company_alltime_distance @companyId INT,@limit BIGINT = 100, @page INT = 1 AS
  BEGIN
    SELECT
      position,
      user_id,
      name,
      surname,
      company_id,
      company_name,
      total_shared_distance as value
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY shared_distance_alltime_rank ) AS position,
            *
          FROM leaderboards_user_company
      where company_id = @companyId
         ) AS RowConstrainedResult
    WHERE position >= ((@page - 1) * @limit + 1)
          AND position < (@page * @limit + 1)
    ORDER BY position
  END
go

CREATE PROCEDURE get_users_leaderboard_company_alltime_shares @companyId INT,@limit BIGINT = 100, @page INT = 1 AS
  BEGIN
    SELECT
      position,
      user_id,
      name,
      surname,
      company_id,
      company_name,
      total_shares as value
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY shared_distance_alltime_rank ) AS position,
            *
          FROM leaderboards_user_company
          where company_id = @companyId
         ) AS RowConstrainedResult
    WHERE position >= ((@page - 1) * @limit + 1)
          AND position < (@page * @limit + 1)
    ORDER BY position
  END
go

CREATE PROCEDURE get_users_leaderboard_company_monthly_shares  @companyId INT, @limit BIGINT = 100, @page INT = 1 AS
  BEGIN
    SELECT
      position,
      user_id,
      name,
      surname,
      company_id,
      company_name,
      monthly_shares as value
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY shared_distance_monthly_rank ) AS position,
            *
          FROM leaderboards_user_company
          where company_id = @companyId
         ) AS RowConstrainedResult
    WHERE position >= ((@page - 1) * @limit + 1)
          AND position < (@page * @limit + 1)
    ORDER BY position
  END
go

CREATE PROCEDURE get_users_leaderboard_company_weekly_shares @companyId INT, @limit BIGINT = 100, @page INT = 1 AS
  BEGIN
    SELECT
      position,
      user_id,
      name,
      surname,
      company_id,
      company_name,
      weekly_shares as value
    FROM (SELECT
            ROW_NUMBER()
            OVER (
              ORDER BY shared_distance_weekly_rank ) AS position,
            *
          FROM leaderboards_user_company
          where company_id = @companyId
         ) AS RowConstrainedResult
    WHERE position >= ((@page - 1) * @limit + 1)
          AND position < (@page * @limit + 1)
    ORDER BY position
  END
go

CREATE PROCEDURE get_user_status @uID INT AS
  BEGIN
    SELECT TOP 1 *
    FROM user_status
    WHERE user_id = @uID
    ORDER BY date DESC
  END
go

