DROP DATABASE IF EXISTS SongsDb;
CREATE DATABASE SongsDb CHARSET 'utf8';
USE SongsDb;

create table Songs(
	SongId integer unsigned not null auto_increment primary key,
    Name varchar(200) not null,
    Artist varchar(200) not null
);

create table Fingerprints(
	Hash bigint not null,
	SongId integer unsigned NOT NULL,
	Offset int unsigned NOT NULL,
    index(Hash),
    foreign key(SongId) references Songs(SongId)
);