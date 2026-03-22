DELETE from Appointments

DBCC CHECKIDENT ('Appointments', RESEED, 0);   