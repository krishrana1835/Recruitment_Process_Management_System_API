-- ## 1. Populate Roles Table
-- Populating with roles defined in the requirements document[cite: 49, 50, 51, 52, 53, 54, 55, 56].
INSERT INTO Roles (role_name) VALUES
('Admin'),
('Recruiter'),
('HR'),
('Interviewer'),
('Reviewer'),
('Candidate'),
('Viewer');

select * from Roles

--------------------------------------------------------------------------------

-- ## 2. Populate Users Table
-- Creating users to fill the roles above. USER0001 is the Super Admin.
INSERT INTO Users (user_id, name, email, password, created_at) VALUES
('USER0001', 'Admin User', 'admin@example.com', 'hashed_password_xyz', '2025-01-10'),
('USER0002', 'Rebecca Field', 'rebecca.f@example.com', 'hashed_password_abc', '2025-01-15'),
('USER0003', 'Henry Mills', 'henry.m@example.com', 'hashed_password_ghi', '2025-03-10'),
('USER0004', 'David Nolan', 'david.n@example.com', 'hashed_password_jkl', '2025-04-05'),
('USER0005', 'Emma Swan', 'emma.s@example.com', 'hashed_password_mno', '2025-05-01'),
('USER0006', 'Mary Blanchard', 'mary.b@example.com', 'hashed_password_pqr', '2025-05-15');

--------------------------------------------------------------------------------

-- ## 3. Populate Users_Roles Table
-- Assigning the newly created roles to the users.
INSERT INTO Users_Roles (role_id, user_id) VALUES
(1, 'USER0001'), -- Admin User is a Super Admin
(2, 'USER0002'), -- Rebecca Field is a Recruiter
(3, 'USER0003'), -- Henry Mills is in HR
(4, 'USER0004'), -- David Nolan is an Interviewer
(5, 'USER0005'), -- Emma Swan is a Reviewer
(4, 'USER0006'); -- Mary Blanchard is also an Interviewer

select * from Users_Roles

--------------------------------------------------------------------------------

-- ## 4. Populate Skills Table
-- Note: The schema specifies VARCHAR(1). 'P' for Python, 'J' for Java, etc.
INSERT INTO Skills (skill_name) VALUES
('Python'), -- Python
('Java'), -- Java
('SQL'), -- SQL
('React'), -- React
('AWS'), -- AWS
('C#'), -- C#
('TypeScript'); -- TypeScript

select * from Skills
delete from skills
DBCC CHECKIDENT ('Skills', RESEED, 0);

--------------------------------------------------------------------------------

-- ## 5. Populate Candidates Table
-- Manually creating candidate profiles[cite: 12].
INSERT INTO Candidates (candidate_id, full_name, email, phone, resume_path, created_at) VALUES
('CAND0001', 'Alice Johnson', 'alice.j@email.com', '5551234567', '/resumes/alice_johnson.pdf', '2025-06-01'),
('CAND0002', 'Bob Williams', 'bob.w@email.com', '5552345678', '/resumes/bob_williams.pdf', '2025-06-02'),
('CAND0003', 'Charlie Davis', 'charlie.d@email.com', '5553456789', '/resumes/charlie_davis.pdf', '2025-06-03'),
('CAND0004', 'Diana Miller', 'diana.m@email.com', '5554567890', '/resumes/diana_miller.pdf', '2025-06-04'),
('CAND0005', 'Evan Garcia', 'evan.g@email.com', '5555678901', '/resumes/evan_garcia.pdf', '2025-06-05');

select * from Candidates

--------------------------------------------------------------------------------

-- ## 6. Populate Candidate_Skills Table
-- Specifying years of experience for candidate skills[cite: 18].
INSERT INTO Candidate_Skills (years_experience, skill_id, candidate_id) VALUES
 -- Alice: 5 years Python
 -- Alice: 4 years SQL
(2, 6, 'CAND0002'), -- Bob: 2 years TypeScript
(6, 7, 'CAND0003'), -- Charlie: 6 years AWS
(4, 1, 'CAND0004'); -- Diana: 4 years Python

select * from Candidate_Skills

--------------------------------------------------------------------------------

-- ## 7. Populate Jobs_Status Table
INSERT INTO Jobs_Status ( status, reason, changed_at, changed_by) VALUES
('Open', 'New role approved', '2025-05-10', 'USER0002'),
('Open', 'New role approved', '2025-05-11', 'USER0002'),
('Closed', 'Position filled by Alice Johnson', '2025-08-01', 'USER0002'),
('On Hold', 'Budgetary review', '2025-06-15', 'USER0001'),
('Open', 'Team expansion', '2025-05-20', 'USER0002');

select * from Jobs_Status
EXEc sp_help 'Jobs_Status'
DBCC CHECKIDENT ('Skills', RESEED, 0);
EXEC sp_rename 'Jobs_Status.changed_by_user_id', 'changed_by', 'COLUMN';


--------------------------------------------------------------------------------

-- ## 8. Populate Jobs Table
-- Recruiter USER0002 creates job openings.
INSERT INTO Jobs (job_title, job_description, created_at, created_by, status_id) VALUES
('Senior Backend Engineer', 'Responsible for server-side logic and database integration.', '2025-05-10', 'USER0002', 1),
('Frontend Developer', 'Develop user-facing features with React and TypeScript.', '2025-05-11', 'USER0002', 2),
('DevOps Specialist', 'Manage CI/CD pipelines and cloud infrastructure on AWS.', '2025-05-12', 'USER0002', 1),
('QA Automation Engineer', 'Create and maintain automated testing frameworks.', '2025-05-15', 'USER0002', 4),
('Product Manager', 'Define product vision and manage the product lifecycle.', '2025-05-20', 'USER0002', 5);

DBCC CHECKIDENT ('Jobs', RESEED, 0);
select * from Jobs
delete from Jobs
EXEc sp_help 'Jobs'
EXEC sp_rename 'Jobs.created_by_user_id', 'created_by', 'COLUMN';


--------------------------------------------------------------------------------

-- ## 9. Populate Jobs_Skills Table
-- Defining required and preferred skills for jobs[cite: 4]. Skill Type 'R' = Required, 'P' = Preferred.
INSERT INTO Jobs_Skills (skill_type, skill_id, job_id) VALUES
('R', 1, 1), -- Python for Backend Engineer
('R', 3, 1), -- SQL for Backend Engineer
('P', 5, 1), -- AWS for Backend Engineer
('R', 4, 2), -- React for Frontend Developer
('R', 7, 2), -- TypeScript for Frontend Developer
('R', 5, 3); -- AWS for DevOps Specialist

DBCC CHECKIDENT ('Jobs', RESEED, 0);
select * from Jobs
delete from Jobs
EXEc sp_help 'Jobs_Skills'
EXEC sp_rename 'Jobs.created_by_user_id', 'created_by', 'COLUMN';

--------------------------------------------------------------------------------

-- ## 10. Populate Interview_Type Table
-- Defining different interview rounds[cite: 25, 26].
INSERT INTO Interview_Type (interview_round_name, process_descreption) VALUES
('HR Screening', 'Initial call to assess culture fit and basic qualifications.'),
('Technical Phone Screen', 'A one-hour technical interview over the phone or video call.'),
('On-site Technical Panel', 'In-depth technical assessment with multiple team members.'),
('Hiring Manager Round', 'Discussion with the hiring manager about the role and team dynamics.'),
('Final HR Round', 'Final discussion regarding compensation and offer details.');

DBCC CHECKIDENT ('Interview_Type', RESEED, 0);
select * from Interview_Type
delete from Interview_Type
EXEc sp_help 'Interview_Type'
EXEC sp_rename 'Interview_type.process_description', 'process_descreption', 'COLUMN';

--------------------------------------------------------------------------------

-- ## 11. Populate Interviews Table
-- Recruiter USER0002 schedules interviews for candidates.
INSERT INTO Interviews (scheduled_at, round_number, location_or_link, candidate_id, job_id, scheduled_by, interview_type_id) VALUES
('2025-06-10', 1, 'https://zoom.us/j/1234567890', 'CAND0001', 1, 'USER0002', 2),
('2025-06-12', 1, 'https://zoom.us/j/2345678901', 'CAND0002', 2, 'USER0002', 2),
('2025-06-18', 2, 'Office A, Room 301', 'CAND0001', 1, 'USER0002', 3),
('2025-06-25', 2, 'https://zoom.us/j/4567890123', 'CAND0001', 1, 'USER0003', 5),
('2025-06-20', 1, 'Phone Call', 'CAND0003', 3, 'USER0002', 1);

DBCC CHECKIDENT ('Interview_Type', RESEED, 0);
select * from Interviews
delete from Interview_Type
EXEc sp_help 'Interviews'
EXEC sp_rename 'Interviews.scheduled_by_user_id', 'scheduled_by', 'COLUMN';

--------------------------------------------------------------------------------

-- ## 12. Populate Interview_Panel Table
-- Setting up a panel interview with multiple interviewers for interview_id 3[cite: 30].
INSERT INTO Interview_Panel (user_id, interview_id) VALUES
('USER0004', 1), -- David interviews for interview 1
('USER0006', 2), -- Mary interviews for interview 2
('USER0004', 3), -- David is on the panel for interview 3
('USER0006', 3), -- Mary is also on the panel for interview 3
('USER0003', 4); -- Henry (HR) conducts the final HR round

--------------------------------------------------------------------------------

-- ## 13. Populate Interview_Feedback Table
-- Interviewers provide feedback with ratings against technologies and comments[cite: 32, 33, 34, 35].
INSERT INTO Interview_Feedback (rating, comments, feedback_at, interview_id, user_id, skill_id) VALUES
(4, 'Solved the coding problem efficiently. Strong algorithmic thinking.', '2025-06-10', 1, 'USER0004', 1),
(3, 'Good understanding of component lifecycle but needs more experience with state management.', '2025-06-12', 2, 'USER0006', 4),
(5, 'Excellent system design explanation for a scalable architecture.', '2025-06-18', 3, 'USER0004', 5),
(4, 'Strong grasp of database normalization and query optimization.', '2025-06-18', 3, 'USER0006', 3),
(5, 'Excellent culture fit, clear communication and salary expectations are within range.', '2025-06-25', 4, 'USER0003', 1);

DBCC CHECKIDENT ('Interview_Feedback', RESEED, 0);
select * from Interview_Feedback
delete from Interview_Feedback
EXEc sp_help 'Interview_Feedback'
EXEC sp_rename 'Interview_Feedback.scheduled_by_user_id', 'scheduled_by', 'COLUMN';

--------------------------------------------------------------------------------

-- ## 14. Populate Candidate_Status_History Table
-- Tracking candidate progress through the recruitment stages.
INSERT INTO Candidate_Status_History (status, reason, changed_at, candidate_id, job_id, changed_by) VALUES
('Applied', 'Candidate applied online', '2025-06-01', 'CAND0001', 1, 'USER0002'),
('Screening', 'Resume shortlisted by reviewer Emma Swan.', '2025-06-05', 'CAND0001', 1, 'USER0005'),
('Interviewing', 'Moved to interview stage after successful screening[cite: 20].', '2025-06-08', 'CAND0001', 1, 'USER0002'),
('Rejected', 'Not a good fit for the role after screening.', '2025-06-06', 'CAND0004', 1, 'USER0005'),
('Offered', 'Offer extended after positive final round feedback.', '2025-07-01', 'CAND0001', 1, 'USER0003'),
('Hired', 'Candidate accepted the offer.', '2025-07-07', 'CAND0001', 1, 'USER0003');

DBCC CHECKIDENT ('Candidate_Status_History', RESEED, 0);
select * from Candidate_Status_History
delete from Candidate_Status_History
EXEc sp_help 'Candidate_Status_History'
EXEC sp_rename 'Candidate_Status_History.changed_by_user_id', 'changed_by', 'COLUMN';

--------------------------------------------------------------------------------

-- ## 15. Populate Candidate_Documents Table
-- Hired candidate uploads documents for verification[cite: 39].
INSERT INTO Candidate_Documents (document_type, file_path, verification_status, uploaded_at, candidate_id) VALUES
('Resume', '/docs/CAND0001/resume.pdf', 'Verified', '2025-06-01', 'CAND0001'),
('ID Proof', '/docs/CAND0001/id.pdf', 'Verified', '2025-07-05', 'CAND0001'),
('Education Certificate', '/docs/CAND0001/degree.pdf', 'Pending', '2025-07-05', 'CAND0001'),
('Resume', '/docs/CAND0002/resume.pdf', 'Verified', '2025-06-02', 'CAND0002'),
('Portfolio', '/docs/CAND0002/portfolio.zip', 'Not Submitted', '2025-06-02', 'CAND0002');

DBCC CHECKIDENT ('Candidate_Documents', RESEED, 1);
select * from Candidate_Documents
delete from Candidate_Documents
EXEc sp_help 'Candidate_Documents'
EXEC sp_rename 'Candidate_Documents.changed_by_user_id', 'changed_by', 'COLUMN';

--------------------------------------------------------------------------------

-- ## 16. Populate Employee_Records Table
-- Profile of the hired candidate is moved to employee records with joining date[cite: 41, 42].
INSERT INTO Employee_Records (employee_id, joining_date, offer_letter_path, candidate_id, job_id) VALUES
('EMP00001', '2025-07-20', '/offers/CAND0001_offer.pdf', 'CAND0001', 1);

--------------------------------------------------------------------------------

-- ## 17. Populate Notifications Table
-- Relevant stakeholders are notified of progress[cite: 46].
INSERT INTO Notifications ( message, status, created_at, user_id) VALUES
('New application for Senior Backend Engineer from Alice Johnson.', 'Sent', '2025-06-01', 'USER0002'),
('ACTION REQ: Screen resume for CAND0001.', 'Sent', '2025-06-02', 'USER0005'),
('Interview for CAND0001 has been scheduled.', 'Sent', '2025-06-08', 'USER0004'),
('Feedback submitted for Alice Johnson by David Nolan.', 'Sent', '2025-06-18', 'USER0002'),
('Offer letter sent to Alice Johnson.', 'Sent', '2025-07-01', 'USER0003');

DBCC CHECKIDENT ('Notifications', RESEED, 1);
select * from Notifications
delete from Notifications
EXEc sp_help 'Notifications'
EXEC sp_rename 'Notifications.changed_by_user_id', 'changed_by', 'COLUMN';

--------------------------------------------------------------------------------

-- ## 18. Populate Candidate_Reviews Table
-- Reviewer adds comments during the screening stage[cite: 17].
INSERT INTO Candidate_Reviews (comments, reviewed_at, candidate_id, job_id, user_id) VALUES
('Strong profile with 5 years in Python and relevant project experience. Shortlisted.', '2025-06-05', 'CAND0001', 1, 'USER0005'),
('Good frontend skills, but lacks the required years of experience in TypeScript. Rejecting.', '2025-06-06', 'CAND0002', 2, 'USER0005'),
('Experience does not align with our current DevOps needs. Not a fit.', '2025-06-07', 'CAND0003', 3, 'USER0005');

DBCC CHECKIDENT ('Candidate_Reviews', RESEED, 1);
select * from Candidate_Reviews
delete from Candidate_Reviews
EXEc sp_help 'Candidate_Reviews'
EXEC sp_rename 'Candidate_Reviews.changed_by_user_id', 'changed_by', 'COLUMN';