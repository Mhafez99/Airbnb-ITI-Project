using Airbnb.Db;
using Airbnb.DTOs;
using Airbnb.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Airbnb.Services
{
    public class OrderService
    {
        private readonly BookingContext _context;

        public OrderService(BookingContext context)
        {
            _context = context;
        }
        public async Task<List<Order>> QueryAsync(FilterOrderDto filter)
        {
            var query = _context.Orders
                .Include(o => o.Stay)
                .Include(o => o.Host)
                .Include(o => o.Buyer)
                .Include(o=>o.Guests)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Term))
            {
                query = query.Where(o =>
                    o.Stay.Name.Contains(filter.Term) ||
                    o.Host.Fullname.Contains(filter.Term));
            }

            if (filter.HostId.HasValue)
                query = query.Where(o => o.Host.Id == filter.HostId);

            if (filter.BuyerId.HasValue)
                query = query.Where(o => o.Buyer.Id == filter.BuyerId);

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(o => o.Status == filter.Status);

            if (!string.IsNullOrEmpty(filter.StayName))
                query = query.Where(o => o.Stay.Name == filter.StayName.Replace("amp;", "&"));

            if (!string.IsNullOrEmpty(filter.HostName))
                query = query.Where(o => o.Host.Fullname == filter.HostName);

            if (filter.TotalPrice.HasValue)
                query = query.Where(o => o.TotalPrice == filter.TotalPrice);

            return await query.ToListAsync();
        }

        public async Task<Order> AddAsync(Order order)
        {
            var stay = await _context.Stays.FindAsync(order.StayId);

            if (stay == null)
                throw new Exception("Stay not found");

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(s => s.Host)
                .Include(s => s.Buyer)
                .Include(s => s.Stay)
                .Include(o => o.Guests)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<Order> UpdateAsync(Order order)
        {
            var existing = await _context.Orders.FindAsync(order.Id);
            if (existing == null) throw new Exception("Order not found");

            _context.Entry(existing).CurrentValues.SetValues(order);
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}