CREATE SCHEMA WebForms;

CREATE TABLE WebForms.Users (
    Id SERIAL PRIMARY KEY,
    Username text NOT NULL,
    PasswordHash text NOT NULL,
    Salt text NOT NULL
);