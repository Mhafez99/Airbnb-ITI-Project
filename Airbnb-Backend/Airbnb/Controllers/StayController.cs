using Airbnb.DTOs;
using Airbnb.Models;
using Airbnb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StayController : ControllerBase
    {
        private readonly StayService _stayService;
        private readonly ILogger<StayController> _logger;

        public StayController(StayService stayService, ILogger<StayController> logger)
        {
            _stayService = stayService;
            _logger = logger;
        }
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            return string.IsNullOrEmpty(userIdClaim) ? null : int.Parse(userIdClaim);
        }

        // GET: api/stay?page=0&Place=xyz&HostId=123
        [HttpGet]
        public async Task<IActionResult> GetStays([FromQuery] StayFilterDto filter, [FromQuery] int page = 0)
        {
            try
            {
                _logger.LogDebug("Getting stays with filter {@filter} and page {page}", filter, page);
                var stays = await _stayService.QueryAsync(filter, page);

                var response = stays.Select(stay => new StayDto
                {
                    Id = stay.Id,
                    Type = stay.Type,
                    Name = stay.Name,
                    Price = stay.Price,
                    Summary = stay.Summary,
                    Capacity = stay.Capacity,
                    HostId = stay.HostId,
                    RoomType = stay.RoomType,
                    Bathrooms = stay.Bathrooms,
                    Bedrooms = stay.Bedrooms,
                    Loc = new LocDto
                    {
                        Country = stay?.Loc?.Country,
                        CountryCode = stay?.Loc?.CountryCode,
                        City = stay?.Loc?.City,
                        Address = stay?.Loc?.Address,
                        Lat = stay?.Loc?.Lat,
                        Lan = stay?.Loc?.Lan
                    },
                    Host = new UserDto
                    {
                        Id = stay.Host.Id,
                        Fullname = stay.Host.Fullname,
                        ThumbnailUrl = stay.Host.ImgUrl
                    },
                    ImgUrls = stay.ImgUrls.Select(i => i.Url).ToList(),
                    Labels = stay.Labels.Select(i => i.Value).ToList(),
                    Amenities = stay.Amenities.Select(a => a.Name).ToList(),
                    LikedByUsers = stay.LikedByUsers.Select(a => a.UserId).ToList(),
                    Reviews = stay?.Reviews?.Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        Txt = r.Txt,
                        Rate = r.Rate,
                        By = new ReviewerDto
                        {
                            Id = r.ReviewerId,
                            Fullname = r.ReviewerFullname,
                            ImgUrl = r.ReviewerImgUrl
                        },
                        StatReviews = r.StatReviews != null ? new StatReviewsDto
                        {
                            Accuracy = r.StatReviews.Accuracy,
                            CheckIn = r.StatReviews.CheckIn,
                            Cleanliness = r.StatReviews.Cleanliness,
                            Communication = r.StatReviews.Communication,
                            Location = r.StatReviews.Location,
                            Value = r.StatReviews.Value
                        } : new StatReviewsDto 
                        {
                            Accuracy = 5,
                            CheckIn = 5,
                            Cleanliness = 5,
                            Communication = 5,
                            Location = 5,
                            Value = 5
                        }
                    }).ToList()
                }).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get stays");
                return StatusCode(500, new { err = "Failed to get stays" });
            }
        }
        // GET: api/stay/length
        [HttpGet("length")]
        public async Task<IActionResult> GetStaysLength([FromQuery] StayFilterDto filter)
        {
            try
            {
                _logger.LogDebug("Getting stays length with filter {@filter}", filter);
                var length = await _stayService.StaysLengthAsync(filter);
                return Ok(length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get stays length");
                return StatusCode(500, new { err = "Failed to get stays length" });
            }
        }
        // GET: api/stay/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStayById(int id)
        {
            try
            {
                var stay = await _stayService.GetByIdAsync(id);
                if (stay == null) return NotFound(new { err = "Stay not found" });
                var response = new StayDto
                {
                    Id = stay.Id,
                    Type = stay.Type,
                    Name = stay.Name,
                    Price = stay.Price,
                    Summary = stay.Summary,
                    Capacity = stay.Capacity,
                    HostId = stay.HostId,
                    RoomType = stay.RoomType,
                    Bathrooms = stay.Bathrooms,
                    Bedrooms = stay.Bedrooms,
                    Loc = new LocDto
                    {
                        Country = stay?.Loc?.Country,
                        CountryCode = stay?.Loc?.CountryCode,
                        City = stay?.Loc?.City,
                        Address = stay?.Loc?.Address,
                        Lat = stay?.Loc?.Lat,
                        Lan = stay?.Loc?.Lan
                    },
                    Host = new UserDto
                    {
                        Id = stay.Host.Id,
                        Fullname = stay.Host.Fullname,
                        ThumbnailUrl = stay.Host.ImgUrl
                    },
                    ImgUrls = stay.ImgUrls.Select(i => i.Url).ToList(),
                    Labels = stay.Labels.Select(i => i.Value).ToList(),
                    Amenities = stay.Amenities.Select(a => a.Name).ToList(),
                    LikedByUsers = stay.LikedByUsers.Select(a => a.UserId).ToList(),

                    Reviews = stay?.Reviews?.Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        Txt = r.Txt,
                        Rate = r.Rate,
                        By = new ReviewerDto
                        {
                            Id = r.ReviewerId,
                            Fullname = r.ReviewerFullname,
                            ImgUrl = r.ReviewerImgUrl
                        },
                        StatReviews = r.StatReviews != null ? new StatReviewsDto
                        {
                            Accuracy = r.StatReviews.Accuracy,
                            CheckIn = r.StatReviews.CheckIn,
                            Cleanliness = r.StatReviews.Cleanliness,
                            Communication = r.StatReviews.Communication,
                            Location = r.StatReviews.Location,
                            Value = r.StatReviews.Value
                        } : new StatReviewsDto
                        {
                            Accuracy = 5,
                            CheckIn = 5,
                            Cleanliness = 5,
                            Communication = 5,
                            Location = 5,
                            Value = 5
                        }
                    }).ToList()
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get stay by id {id}", id);
                return StatusCode(500, new { err = "Failed to get stay" });
            }
        }
        // POST: api/stay
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddStay([FromBody] StayCreateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null) return Unauthorized();
                var stay = new Stay
                {
                    Type = dto.Type,
                    Name = dto.Name,
                    Price = dto.Price,
                    Summary = dto.Summary,
                    Capacity = dto.Capacity,
                    HostId = userId.Value,
                    RoomType = dto.RoomType,
                    Bathrooms = dto.Bathrooms,
                    Bedrooms = dto.Bedrooms,
                    ImgUrls = dto.ImgUrls.Select(url => new StayImage { Url = url }).ToList(),
                };
                if (dto.Loc != null)
                {
                    var loc = new Loc
                    {
                        Country = dto.Loc.Country,
                        CountryCode = dto.Loc.CountryCode,
                        City = dto.Loc.City,
                        Address = dto.Loc.Address,
                        Lat = dto.Loc.Lat,
                        Lan = dto.Loc.Lan,
                        Stay = stay 
                    };
                    stay.Loc = loc;
                }
                if (dto.Amenities != null && dto.Amenities.Any())
                {
                    var amenities = dto.Amenities.Select(name => new Amenity
                    {
                        Name = name,
                        Stay = stay 
                    }).ToList();

                    stay.Amenities = amenities;
                }
                if (dto.Labels != null && dto.Labels.Any())
                {
                    stay.Labels = dto.Labels.Select(name => new Label
                    {
                        Value = name,
                        Stay = stay 
                    }).ToList();
                }
                
                stay.LikedByUsers = [];

                stay.Reviews = new List<Review>();

                var addedStay = await _stayService.AddAsync(stay);

                var response = new StayDto
                {
                    Id = addedStay.Id,
                    Type = addedStay.Type,
                    Name = addedStay.Name,
                    Price = addedStay.Price,
                    Summary = addedStay.Summary,
                    Capacity = addedStay.Capacity,
                    HostId = addedStay.HostId,
                    RoomType = addedStay.RoomType,
                    Bathrooms = addedStay.Bathrooms,
                    Bedrooms = addedStay.Bedrooms,
                    Loc = addedStay.Loc != null ? new LocDto
                    {
                        Country = addedStay.Loc.Country,
                        CountryCode = addedStay.Loc.CountryCode,
                        City = addedStay.Loc.City,
                        Address = addedStay.Loc.Address,
                        Lat = addedStay.Loc.Lat,
                        Lan = addedStay.Loc.Lan
                    } : null,
                    Labels = addedStay.Labels.Select(i=>i.Value).ToList(),
                    ImgUrls = addedStay.ImgUrls.Select(i => i.Url).ToList(),
                    Amenities = addedStay.Amenities.Select(a => a.Name).ToList(),
                    LikedByUsers = [],
                    Reviews = new List<ReviewDto>()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add stay");
                return StatusCode(500, new { err = "Failed to add stay" });
            }
        }

        // PUT: api/stay
        [HttpPut]
            public async Task<IActionResult> UpdateStay([FromBody] StayUpdateDto dto)
            {
                try
                {
                    var userId = GetCurrentUserId();
                    if (userId == null) return Unauthorized();
                    var stay = await _stayService.GetByIdAsync(dto.Id);
                    if (stay == null) return NotFound(new { err = "Stay not found" });
                    if (stay.HostId != userId.Value) return Forbid();

                    stay.Type = dto.Type ;
                    stay.Name = dto.Name ;
                    stay.Price = dto.Price ;
                    stay.Summary = dto.Summary ;
                    stay.Capacity = dto.Capacity ;
                    stay.RoomType = dto.RoomType ;
                    stay.Bathrooms = dto.Bathrooms ;
                    stay.Bedrooms = dto.Bedrooms;
                    

                if (dto.Loc != null)
                {
                    if (stay.Loc == null)
                    {
                        stay.Loc = new Loc
                        {
                            Country = dto.Loc.Country,
                            CountryCode = dto.Loc.CountryCode,
                            City = dto.Loc.City,
                            Address = dto.Loc.Address,
                            Lat = dto.Loc.Lat,
                            Lan = dto.Loc.Lan,
                            Stay = stay
                        };
                    }
                    else
                    {
                        stay.Loc.Country = dto.Loc.Country ?? stay.Loc.Country;
                        stay.Loc.CountryCode = dto.Loc.CountryCode ?? stay.Loc.CountryCode;
                        stay.Loc.City = dto.Loc.City ?? stay.Loc.City;
                        stay.Loc.Address = dto.Loc.Address ?? stay.Loc.Address;
                        stay.Loc.Lat = dto.Loc.Lat != 0 ? dto.Loc.Lat : stay.Loc.Lat;
                        stay.Loc.Lan = dto.Loc.Lan != 0 ? dto.Loc.Lan : stay.Loc.Lan;
                    }
                }
                if (dto.ImgUrls != null && dto.ImgUrls.Any())
                {
                    stay.ImgUrls = dto.ImgUrls.Select(url => new StayImage { Url = url }).ToList();
                }
                if (dto.Amenities != null)
                {
                    stay.Amenities.Clear();
                    stay.Amenities = dto.Amenities
                        .Select(name => new Amenity { Name = name, Stay = stay })
                        .ToList();
                }
                if (dto.Labels != null)
                {
                    stay.Labels.Clear();
                    stay.Labels = dto.Labels
                        .Select(value => new Label { Value = value, Stay = stay })
                        .ToList();
                }

                var updatedStay = await _stayService.UpdateAsync(stay);
                    var response = new StayDto
                    {
                        Id = updatedStay.Id,
                        Type = updatedStay.Type,
                        Name = updatedStay.Name,
                        Price = updatedStay.Price,
                        Summary = updatedStay.Summary,
                        Capacity = updatedStay.Capacity,
                        HostId = userId.Value,
                        RoomType = updatedStay.RoomType,
                        Bathrooms = updatedStay.Bathrooms,
                        Bedrooms = updatedStay.Bedrooms,
                        Loc = new LocDto
                        {
                            Country = updatedStay.Loc.Country,
                            CountryCode = updatedStay.Loc.CountryCode,
                            City = updatedStay.Loc.City,
                            Address = updatedStay.Loc.Address,
                            Lat = updatedStay.Loc.Lat,
                            Lan = updatedStay.Loc.Lan
                        },
                        ImgUrls = updatedStay.ImgUrls.Select(i => i.Url).ToList(),
                        Labels = stay.Labels.Select(i => i.Value).ToList(),
                        Amenities = stay.Amenities.Select(a => a.Name).ToList(),
                    };

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update stay");
                    return StatusCode(500, new { err = "Failed to update stay" });
                }
            }

        [HttpPost("{stayId}/like")]
        public async Task<IActionResult> ToggleLike(int stayId, [FromBody] LikedByUserDto dto)
        {
            try
            {
                var stay = await _stayService.ToggleLikeAsync(stayId, dto.UserId);
                var response = new StayDto
                {
                    Id = stay.Id,
                    Name = stay.Name,
                    LikedByUsers = stay.LikedByUsers.Select(u => u.UserId).ToList(),
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("{stayId}/reviews")]
        public async Task<IActionResult> AddReview(int stayId, [FromBody] ReviewDto dto)
        {
            var review = new Review
            {
                At = DateTime.UtcNow,
                Txt = dto.Txt,
                Rate = dto.Rate,
                ReviewerId = dto.By.Id,
                ReviewerFullname = dto.By.Fullname,
                ReviewerImgUrl = dto.By.ImgUrl,
                StayId = stayId,
                StatReviews = new StatReviews
                {
                    Accuracy = dto.StatReviews.Accuracy,
                    CheckIn = dto.StatReviews.CheckIn,
                    Cleanliness = dto.StatReviews.Cleanliness,
                    Communication = dto.StatReviews.Communication,
                    Location = dto.StatReviews.Location,
                    Value = dto.StatReviews.Value
                }
            };
            var addedReview = await _stayService.AddReviewAsync(stayId, review);

            if (addedReview == null) return NotFound(new { err = "Stay not found" });

            var response = new ReviewDto
            {
                Txt = dto.Txt,
                Rate = dto.Rate,
                By = new ReviewerDto
                {
                    Id = dto.By.Id,
                    Fullname = dto.By.Fullname,
                    ImgUrl = dto.By.ImgUrl
                },
                StatReviews = new StatReviewsDto
                {
                    Accuracy = dto.StatReviews.Accuracy,
                    CheckIn = dto.StatReviews.CheckIn,
                    Cleanliness = dto.StatReviews.Cleanliness,
                    Communication = dto.StatReviews.Communication,
                    Location = dto.StatReviews.Location,
                    Value = dto.StatReviews.Value
                }
            };
            return Ok(response);

        }


        //DELETE: api/stay/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStay(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null) return Unauthorized();

                var stay = await _stayService.GetByIdAsync(id);
                if (stay == null) return NotFound(new { err = "Stay not found" });
                if (stay.HostId != userId.Value) return Forbid(); 

                await _stayService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete stay {id}", id);
                return StatusCode(500, new { err = "Failed to delete stay" });
            }
        }
    }

}
