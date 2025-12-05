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
                interview_round_name = r.interview_round_name,
                interview_type_id = r.interview_type_id,
                process_descreption = r.process_descreption,
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
                interview_round_name = dto.interview_round_name,
                process_descreption = dto.process_descreption,
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

            var data = await _context.Interview_Types.FirstOrDefaultAsync(r => r.interview_type_id == dto.interview_type_id) ?? throw new NullReferenceException("Interview Type Not found");

            data.interview_round_name = dto.interview_round_name;
            data.process_descreption = dto.process_descreption;

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<Boolean> DeleteInterviewType(Interview_TypeDtos.DeleteInterviewType dto)
        {
            var data = await _context.Interview_Types.FirstOrDefaultAsync(r => r.interview_type_id == dto.interview_type_id) ?? throw new NullReferenceException("Interview Type not found");

            _context.Interview_Types.Remove(data);

           var result =  await _context.SaveChangesAsync();

            return result > 0;
        }
    }
}
