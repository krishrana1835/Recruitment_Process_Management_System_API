using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;
using RecruitmentApi.Models;

namespace RecruitmentApi.Services
{
    public class Interview_TypeService
    {
        private AppDbContext _context;
        public Interview_TypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Interview_TypeDtos.InterviewType>> FetchAllInterviewTypes()
        {
            var data = await _context.Interview_Types.ToListAsync();

            if (data == null)
            {
                return [];
            }

            var response = data.Select(r => new Interview_TypeDtos.InterviewType
            {
                interview_round_name = r.InterviewRoundName,
                interview_type_id = r.InterviewTypeId,
                process_descreption = r.ProcessDescreption,
            }).ToList();

            return response;
        }

        public async Task<Interview_TypeDtos.AddInterviewType> AddNewInterviewType(Interview_TypeDtos.AddInterviewType dto)
        {
            if (dto.interview_round_name.IsNullOrEmpty())
                throw new ArgumentException("Round name is empty or null");

            if (dto.process_descreption.IsNullOrEmpty())
                throw new ArgumentException("Process description is not provided");

            Interview_Type data = new Interview_Type
            {
                InterviewRoundName = dto.interview_round_name,
                ProcessDescreption = dto.process_descreption,
            };

            await _context.Interview_Types.AddAsync(data);

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<Interview_TypeDtos.InterviewType> UpdateInterviewType(Interview_TypeDtos.InterviewType dto)
        {
            if (dto.interview_round_name.IsNullOrEmpty())
                throw new ArgumentException("Round name is empty");

            if (dto.process_descreption.IsNullOrEmpty())
                throw new ArgumentException("process description is empty");

            var data = await _context.Interview_Types.FirstOrDefaultAsync(r => r.InterviewTypeId == dto.interview_type_id) ?? throw new NullReferenceException("Interview Type Not found");

            data.InterviewRoundName = dto.interview_round_name;
            data.ProcessDescreption = dto.process_descreption;

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<Boolean> DeleteInterviewType(Interview_TypeDtos.DeleteInterviewType dto)
        {
            var data = await _context.Interview_Types.FirstOrDefaultAsync(r => r.InterviewTypeId == dto.interview_type_id) ?? throw new NullReferenceException("Interview Type not found");

            _context.Interview_Types.Remove(data);

           var result =  await _context.SaveChangesAsync();

            return result > 0;
        }
    }
}
