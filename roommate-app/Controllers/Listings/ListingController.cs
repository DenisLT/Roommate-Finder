﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Bcpg;
using roommate_app.Exceptions;
using roommate_app.Models;
using roommate_app.Other.ListingComparers;
using roommate_app.Other.Services;
using roommate_app.Services;
using System.Diagnostics.CodeAnalysis;

namespace roommate_app.Controllers.Listings;

[ApiController]
[Route("[controller]")]
[ExcludeFromCodeCoverage]
public class ListingController : Controller
{
    private readonly IListingCompreterFactory _listingFactory;
    private readonly IErrorLogging _errorLogging;
    private readonly IListingService _listingService;
    private readonly IGenericService _genericService;
    private readonly IFavoritesService _favoritesService;
    private readonly IRatingsService _ratingsService;

    private User user;

    public ListingController(
        IListingCompreterFactory listingFactory,
        IErrorLogging errorLogging,
        IListingService listingService,
        IGenericService genericService,
        IFavoritesService favoritesService,
        IRatingsService ratingsService
        )
    {
        _listingFactory = listingFactory;
        _errorLogging = errorLogging;
        _listingService = listingService;
        _genericService = genericService;
        _favoritesService = favoritesService;
        _ratingsService = ratingsService;
    }

    void addDataToListing(Listing listing)
    {
        listing.IsFavorite = _favoritesService.IsFavorite(user.Id, listing.Id);
        listing.RatingCount = _ratingsService.GetRatingsCount(listing.Id);
        if (listing.RatingCount > 0)
            listing.Rating = _ratingsService.GetRating(listing.Id);
        listing.UserHasRated = _ratingsService.UserHasRated(user.Id, listing.Id);
        if (listing.UserHasRated)
            listing.UserRating = _ratingsService.GetUserRating(user.Id, listing.Id);
    }

    [HttpGet]
    [Route("sort")]
    public async Task<JsonResult> GetSortedListings(SortMode sort, string city)
    {
        user = (User)HttpContext.Items["User"];

        var existingListings = await _genericService.GetAllAsync<Listing>();

        var factory = _listingFactory.createListingComparerFactory();
        var comparer = factory.GetComparer(sortMode: sort, city: city);

        existingListings.Sort(comparer);

        if(user != null)
            existingListings.ForEach(l => addDataToListing(l));

        var response = new JsonResult(existingListings);
        response.StatusCode = 200;

        return response;
    }
    [HttpGet]
    [Route("userlistings")]
    public async Task<JsonResult> GetUserListings(int id)
    {
        var existingListings = _listingService.GetByUserId(id);

        var response = new JsonResult(existingListings);
        response.StatusCode = 200;

        return response;
    }

    [HttpPost]
    public async Task<ActionResult> Submit([FromBody] Listing listing)
    {
        List<Listing> existingListings = new List<Listing>();
        List<User> existingUsers = await _genericService.GetAllAsync<User>();
        listing.Date = DateTime.Now.ToString("yyyy-MM-dd");
        User user = existingUsers.Where(u => u.Email == listing.Email).First();

        listing.UserId = user.Id;
        listing.User = user;

        var response = new JsonResult(new object());
        var isInternalServerError = false;
        var message = "";

        try
        {
            await _genericService.AddAsync(listing);
            existingListings.Add(listing);
        }
        catch (ArgumentNullException e)
        {
            _errorLogging.LogError(e.Message);
            _errorLogging.MessageError("Failed to load existing listing");
            isInternalServerError = true;
            message = "Failed to load existing listing";
        }
        catch (SqlException e)
        {
            _errorLogging.LogError(e.Message);
            _errorLogging.MessageError("Could not insert a listing into the database.");
            isInternalServerError = true;
            message = "Could not insert a listing into the database.";
        }
        catch (Exception e)
        {
            _errorLogging.LogError(e.Message);
            _errorLogging.MessageError("Unexpected error, please restart the program");
            isInternalServerError = true;
            message = "Unexpected error, please restart the program";
        }

        if (isInternalServerError)
        {
            response = new JsonResult(message);
            response.StatusCode = 500;
        }
        else
        {
            response = new JsonResult(await _genericService.GetAllAsync<Listing>());
            response.StatusCode = 201;
        }

        return response;
    }

    [HttpPost]
    [Route("update")]
    public async Task<ActionResult> UpdateListing([FromBody] Listing listing)
    {
        var response = new JsonResult(new object());
        var isInternalServerError = false;
        var message = "";

        try
        {
            await _listingService.UpdateAsync(listing.Id, listing);
        }
        catch (SqlException e)
        {
            _errorLogging.LogError(e.Message);
            _errorLogging.MessageError("Could not update a listing (SQL database exception).");
            isInternalServerError = true;
            message = "Could not update a listing (SQL database exception).";
        }
        catch (Exception e)
        {
            _errorLogging.LogError(e.Message);
            _errorLogging.MessageError("Unexpected error, please restart the program");
            isInternalServerError = true;
            message = "Unexpected error, please restart the program";
        }

        if (isInternalServerError)
        {
            response = new JsonResult(message);
            response.StatusCode = 500;
        }
        else
        {
            response = new JsonResult("Listing updated");
            response.StatusCode = 200;
        }

        return response;
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult> DeleteListing([FromBody] Listing listing)
    {
        var response = new JsonResult(new object());
        var isInternalServerError = false;
        var message = "";

        try
        {
            await _genericService.DeleteAsync<Listing>(listing.Id);
        }
        catch (SqlException e)
        {
            _errorLogging.LogError(e.Message);
            _errorLogging.MessageError("Could not update a listing (SQL database exception).");
            isInternalServerError = true;
            message = "Could not update a listing (SQL database exception).";
        }
        catch (Exception e)
        {
            _errorLogging.LogError(e.Message);
            _errorLogging.MessageError("Unexpected error, please restart the program");
            isInternalServerError = true;
            message = "Unexpected error, please restart the program";
        }

        if (isInternalServerError)
        {
            response = new JsonResult(message);
            response.StatusCode = 500;
        }
        else
        {
            response = new JsonResult("Listing deleted");
            response.StatusCode = 200;
        }

        return response;
    }

    [HttpGet]
    [Route("filter")]
    public JsonResult Filter(int lowPrice, int highPrice, string city, int count)
    {
        var listings = _listingService.Filter(lowPrice: lowPrice, highPrice: highPrice, city: city, count: count);

        var response = new JsonResult(listings);
        response.StatusCode = 200;


        return response;
    }
}
