using Airbnb.Db;
using Airbnb.DTOs;
using Airbnb.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Airbnb.Services
{
    public class StayService
    {
        private readonly BookingContext _context;
        private const int STAY_INCREMENT = 20;

        public StayService(BookingContext context)
        {
            _context = context;
        }
        public async Task<List<Stay>> QueryAsync(StayFilterDto filter, int index)
        {
            var query = _context.Stays
                .Include(s => s.Host)
                .Include(s => s.Amenities)
                .Include(s => s.Labels)
                .Include(s => s.ImgUrls)
                .Include(s => s.Orders)
                .Include(s=>s.Loc)
                .Include(s => s.LikedByUsers)
                .Include(s=>s.Reviews)
                   .ThenInclude(r => r.StatReviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Place))
                query = query.Where(s => s.Loc.Address.Contains(filter.Place));

            if (filter.HostId.HasValue)
                query = query.Where(s => s.HostId == filter.HostId.Value);

            if (!string.IsNullOrEmpty(filter.Label))
                query = query.Where(s => s.Labels.Any(l => l.Value == filter.Label));

            if (filter.LikeByUser.HasValue)
            {
                query = query.Where(s =>
                    s.LikedByUsers.Any(lu => lu.UserId == filter.LikeByUser.Value));
            }

            if (filter.IsPetAllowed.HasValue && filter.IsPetAllowed.Value)
                query = query.Where(s => s.Amenities.Any(a => a.Name == "Pets allowed"));

            return await query
                .Skip(STAY_INCREMENT * index)
                .Take(STAY_INCREMENT)
                .ToListAsync();
        }

        public async Task<int> StaysLengthAsync(StayFilterDto filter)
        {
            var query = _context.Stays
                .Include(s => s.Amenities)
                .Include(s => s.Labels)
                .Include(s => s.LikedByUsers)
                .Include(s=>s.Loc)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Place))
                query = query.Where(s => s.Loc.Address.Contains(filter.Place));

            if (filter.HostId.HasValue)
                query = query.Where(s => s.HostId == filter.HostId.Value);

            if (!string.IsNullOrEmpty(filter.Label))
                query = query.Where(s => s.Labels.Any(l => l.Value == filter.Label));


            if (filter.LikeByUser.HasValue)
            {
                query = query.Where(s =>
                    s.LikedByUsers.Any(lu => lu.UserId == filter.LikeByUser.Value));
            }


            if (filter.IsPetAllowed.HasValue && filter.IsPetAllowed.Value)
                query = query.Where(s => s.Amenities.Any(a => a.Name == "Pets allowed"));

            return await query.CountAsync();
        }

        public async Task<Stay?> GetByIdAsync(int id)
        {
            return await _context.Stays
                .Include(s => s.Host)
                .Include(s => s.Amenities)
                .Include(s => s.Labels)
                .Include(s => s.ImgUrls)
                .Include(s => s.LikedByUsers)
                .Include(s => s.Orders)
                .Include(s=>s.Loc)
                .Include(s=>s.Reviews)
                .ThenInclude(s=>s.StatReviews)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Stay> AddAsync(Stay stay)
        {
            _context.Stays.Add(stay);
            await _context.SaveChangesAsync();
            var addedStay = await _context.Stays
                .Include(s => s.Host)
                .Include(s => s.Amenities)
                .Include(s => s.Labels)
                .Include(s => s.ImgUrls)
                .Include(s => s.LikedByUsers)
                .Include(s => s.Orders)
                .Include(s => s.Loc)
                .Include(s => s.Reviews)
                .ThenInclude(s => s.StatReviews)
           .FirstOrDefaultAsync(s => s.Id == stay.Id);
            return addedStay;
        }

        public async Task<Stay> UpdateAsync(Stay stay)
        {
            var existing = await _context.Stays
                .Include(s => s.Host)
                .Include(s => s.Amenities)
                .Include(s => s.Labels)
                .Include(s => s.ImgUrls)
                .Include(s => s.LikedByUsers)
                .Include(s => s.Orders)
                .Include(s => s.Loc)
                .Include(s => s.Reviews)
                .ThenInclude(s => s.StatReviews)
                .FirstOrDefaultAsync(s => s.Id == stay.Id);

            if (existing == null) throw new Exception("Stay not found");

            _context.Entry(existing).CurrentValues.SetValues(stay);
            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<Stay> ToggleLikeAsync(int stayId, int userId)
        {
            var stay = await _context.Stays
                .Include(s => s.LikedByUsers)
                .FirstOrDefaultAsync(s => s.Id == stayId);

            if (stay == null) throw new Exception("Stay not found");

            var existingLike = stay.LikedByUsers.FirstOrDefault(l => l.UserId == userId);

            if (existingLike != null)
            {
                stay.LikedByUsers.Remove(existingLike);
            }
            else
            {
                stay.LikedByUsers.Add(new LikedByUser
                {
                    UserId = userId,
                    StayId = stayId
                });
            }

            await _context.SaveChangesAsync();
            return stay;
        }
        public async Task<Review?> AddReviewAsync(int stayId, Review review)
        {
            var stay = await _context.Stays
                .Include(s => s.Reviews)
                .FirstOrDefaultAsync(s => s.Id == stayId);

            if (stay == null) return null;

            review.StayId = stayId;
            stay.Reviews.Add(review);

            await _context.SaveChangesAsync();
            return review;
        }


        public async Task DeleteAsync(int id)
        {
            var stay = await _context.Stays.FindAsync(id);
            if (stay == null) throw new Exception("Stay not found");

            _context.Stays.Remove(stay);
            await _context.SaveChangesAsync();
        }
    }
}
