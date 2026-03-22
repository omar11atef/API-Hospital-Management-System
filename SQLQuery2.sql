-- 1. السماح بإدخال الـ Id يدوياً في جدول الأقسام
SET IDENTITY_INSERT [dbo].[Departments] ON;

-- 2. تنفيذ عملية الإدخال
INSERT INTO [dbo].[Departments] 
    ([Id], [Name], [Location], [PhoneNumber], [CreatedBy], [CreatedAt], [ModifiedBy], [ModifiedAt])
VALUES 
    (1, N'General Department', N'Main Building', N'0000000000', N'System', GETUTCDATE(), N'System', GETUTCDATE());

-- 3. إعادة إغلاق الإدخال اليدوي لحماية الجدول (خطوة هامة جداً)
SET IDENTITY_INSERT [dbo].[Departments] OFF;