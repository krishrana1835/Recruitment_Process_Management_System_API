namespace RecruitmentApi.Dtos
{
    public class Interview_TypeDtos
    {

        public class InterviewType
        {
            public int interview_type_id { get; set; }

            public string interview_round_name { get; set; } = null!;

            public string? process_descreption { get; set; }
        }

        public class AddInterviewType
        {

            public string interview_round_name { get; set; } = null!;

            public string? process_descreption { get; set; }
        }

        public class DeleteInterviewType
        {
            public int interview_type_id { get; set; }
        }

    }
}
