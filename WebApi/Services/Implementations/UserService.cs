using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;
using DataContracts.Users;
using System;
using WebApi.Exceptions;
using System.Collections.Generic;

namespace WebApi.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserStore _userStore;
        private readonly IHashingService _hashingService;

        public UserService(IUserStore userStore, IHashingService hashingService)
        {
            _userStore = userStore;
            _hashingService = hashingService;
        }

        public async Task AddUser(AddUserRequest userReq)
        {
            IsValidUserRequest(userReq);
            userReq.Salt = Guid.NewGuid().ToString();
            userReq.Password = _hashingService.Hash(userReq.Password, userReq.Salt);
            await _userStore.AddUser(userReq);
        }

        public async Task DeleteUser(string email)
        {
            await _userStore.DeleteUser(email);
        }

        public User GetUser(string email)
        {
            var user = _userStore.GetUser(email);
            return user;
        }

        public async Task UpdateUser(AddUserRequest user)
        {
            IsValidUserRequest(user);
            await _userStore.UpdateUser(user);
        }

        public async Task AddGroup(string groupName)
        {
            if (!IsValidString(groupName))
            {
                throw new BadRequestException($"You need to specify group name");
            }
            await _userStore.AddGroup(groupName);
        }

        private static void IsValidUserRequest(AddUserRequest user)
        {
            if (!(IsValidString(user.FirstName)
                 && IsValidString(user.LastName)
                 && IsValidString(user.Password)))
            {
                throw new BadRequestException("One of the required fields is empty");
            }

            else if (!IsValidEmail(user.Email))
            {
                throw new BadRequestException($"The Email {user.Email} is not a valid AUB Email");
            }

            else if (!IsValidPassword(user.Password))
            {
                throw new BadRequestException("The Password is not valid, it should contain a number, a capital letter, and has a minimum of 8 characters");
            }
            else if (user.GroupId == 0)
            {
                throw new BadRequestException($"You need to secify which group the user belong to");
            }
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var emailValidator = new EmailAddressAttribute();
            bool isValid = emailValidator.IsValid(email);
            var emailParts = email.Split('@');
            if (emailParts.Length != 2 || !emailParts[1].StartsWith("mail.aub.edu"))
            {
                isValid = false;
            }
            return isValid;
        }

        public static bool IsValidPassword(string plainText)
        {
            var hasNumber = CompiledRegex(@"[0-9]+");
            var hasUpperChar = CompiledRegex(@"[A-Z]+");
            var hasMinimum8Chars = CompiledRegex(@".{8,}");
            return hasNumber.IsMatch(plainText) && hasUpperChar.IsMatch(plainText) && hasMinimum8Chars.IsMatch(plainText);
        }

        private static Regex CompiledRegex(string regExp)
        {
            return new Regex(regExp, RegexOptions.Compiled);
        }

        public static bool IsValidString(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

    }
}
