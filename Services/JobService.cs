using Supabase;
using JobTracker.Models;

namespace JobTracker.Services
{
    public class JobService
    {
        private readonly Client _client;

        public JobService(Client client)
        {
            _client = client;
        }

        // CREATE - Add new job
        public async Task<Job?> CreateJobAsync(Job job)
        {
            try
            {
                job.Id = Guid.NewGuid();
                job.CreatedAt = DateTime.UtcNow;

                var response = await _client
                    .From<Job>()
                    .Insert(job);

                return response.Models.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        // READ - Get all jobs for a user
        public async Task<List<Job>> GetJobsByUserAsync(Guid userId)
        {
            try
            {
                var response = await _client
                    .From<Job>()
                    .Where(x => x.UserId == userId)
                    .Get();

                return response.Models;
            }
            catch
            {
                return new List<Job>();
            }
        }

        // READ - Get single job by ID
        public async Task<Job?> GetJobByIdAsync(Guid jobId)
        {
            try
            {
                var response = await _client
                    .From<Job>()
                    .Where(x => x.Id == jobId)
                    .Single();

                return response;
            }
            catch
            {
                return null;
            }
        }

        // READ - Get all jobs (admin only)
        public async Task<List<Job>> GetAllJobsAsync()
        {
            try
            {
                var response = await _client
                    .From<Job>()
                    .Get();

                return response.Models;
            }
            catch
            {
                return new List<Job>();
            }
        }

        // UPDATE - Update existing job
        public async Task<Job?> UpdateJobAsync(Job job)
        {
            try
            {
                var response = await _client
                    .From<Job>()
                    .Where(x => x.Id == job.Id)
                    .Update(job);

                return response.Models.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        // DELETE - Delete job by ID
        public async Task<bool> DeleteJobAsync(Guid jobId)
        {
            try
            {
                await _client
                    .From<Job>()
                    .Where(x => x.Id == jobId)
                    .Delete();

                return true;
            }
            catch
            {
                return false;
            }
        }

        // HELPER - Get jobs by status
        public async Task<List<Job>> GetJobsByStatusAsync(Guid userId, string status)
        {
            try
            {
                var response = await _client
                    .From<Job>()
                    .Where(x => x.UserId == userId && x.Status == status)
                    .Get();

                return response.Models;
            }
            catch
            {
                return new List<Job>();
            }
        }

        // HELPER - Update job status
        public async Task<Job?> UpdateJobStatusAsync(Guid jobId, string newStatus)
        {
            try
            {
                var job = await GetJobByIdAsync(jobId);
                if (job == null) return null;

                job.Status = newStatus;
                return await UpdateJobAsync(job);
            }
            catch
            {
                return null;
            }
        }
    }
}
