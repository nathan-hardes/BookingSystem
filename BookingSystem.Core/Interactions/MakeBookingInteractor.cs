﻿using BookingSystem.Core.Entities;
using BookingSystem.Core.Exceptions;
using BookingSystem.Core.Interfaces;
using BookingSystem.Core.Rules;
using FluentValidation;
using System;

namespace BookingSystem.Core.Interactions
{
    /// <summary>
    /// An interactor for making a booking
    /// </summary>
    public class MakeBookingInteractor
    {
        private IMakeBookingResponseHandler responseHandler;

        private IBookingRepository bookingRepository;

        private IValidator<Booking> validator;

        /// <summary>
        /// Gets or sets the booking to be made
        /// </summary>
        public Booking Booking { get; set; }

        /// <summary>
        /// Creates an interaction that will make a booking against a set space for a given period
        /// </summary>
        /// <param name="responseHandler">The entity responsible for handling the response</param>
        /// <param name="bookingRepository">Repository to search/add the booking to</param>
        /// <param name="validator">Validator used to ensure the booking is valid</param>"
        public MakeBookingInteractor(IMakeBookingResponseHandler responseHandler, IBookingRepository bookingRepository, IValidator<Booking> validator)
        {
            this.responseHandler = responseHandler;
            this.bookingRepository = bookingRepository;
            this.validator = validator;
        }

        /// <summary>
        /// Executes the interactor
        /// </summary>
        public void Execute()
        {
            try
            {
                if (Booking == null)
                    throw new ArgumentException("Booking cannot be null");

                var results = validator.Validate(Booking);

                if (results.IsValid == false)
                    throw new InvalidBookingException();

                if (bookingRepository.HasOverlapping(Booking))
                {
                    responseHandler.Fail("Another booking overlaps with this one.");
                }
                else
                {
                    bookingRepository.Add(Booking);
                    responseHandler.Success();
                }
            }
            catch (Exception e)
            {
                responseHandler.Error(e);
            }
            
            // 2. Ensure that there is no conflict
            // 3. If there is a conflict, log the booking being made
            // 4. If there is a conflict, reject the booking
            // 5. If there is no conflict and the booking is valid, register it
        }
    }
}
