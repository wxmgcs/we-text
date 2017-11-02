﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeText.Common;
using WeText.Common.Events;
using WeText.Domain.Events;

namespace WeText.Domain
{
    public class User : AggregateRoot<Guid>
    {
        private readonly List<Guid> myFriends = new List<Guid>();

        public User()
        {
            ApplyEvent(new UserCreatedEvent(Guid.Empty));
        }

        public User(Guid id)
        {
            ApplyEvent(new UserCreatedEvent(id));
        }

        public User(Guid id, string name, string password, string email, string displayName)
        {
            ApplyEvent(new UserCreatedEvent(id, name, password, email, displayName));
        }

        public string Name { get; private set; }

        public string Password { get; private set; }

        public string DisplayName { get; private set; }

        public string Email { get; private set; }

        public IEnumerable<Guid> MyFriends => myFriends;


        public void ChangeDisplayName(string displayName)
        {
            ApplyEvent(new UserDisplayNameChangedEvent(this.Id, displayName));
        }

        public void ChangeEmail(string email)
        {
            ApplyEvent(new UserEmailChangedEvent(this.Id, email));
        }

        public void SendInvitation(User toUser, string invitationLetter)
        {
            ApplyEvent(new InvitationSentEvent(this.Id, this.Id, toUser.Id, this.DisplayName, toUser.DisplayName, invitationLetter));
        }

        public void ApproveInvitation(Invitation invitation)
        {
            if (invitation.TargetUserId != this.Id)
            {
                throw new InvalidOperationException();
            }

            ApplyEvent(new InvitationApprovedEvent(this.Id, invitation.Id, invitation.OriginatorId, this.Id));
        }

        public void RejectInvitation(Invitation invitation)
        {
            if (invitation.TargetUserId != this.Id)
            {
                throw new InvalidOperationException();
            }

            ApplyEvent(new InvitationRejectedEvent(this.Id, invitation.Id, invitation.OriginatorId, this.Id));
        }

        public void AddFriend(Guid friendUserId)
        {
            ApplyEvent(new FriendAddedEvent(this.Id, friendUserId));
        }

        [InlineEventHandler]
        private void HandleUserCreatedEvent(UserCreatedEvent evnt)
        {
            this.Id = (Guid)evnt.AggregateRootKey;
            this.Password = evnt.Password;
            this.DisplayName = evnt.DisplayName;
            this.Name = evnt.Name;
            this.Email = evnt.Email;
        }

        [InlineEventHandler]
        private void HandleChangeDisplayNameEvent(UserDisplayNameChangedEvent evnt)
        {
            this.DisplayName = evnt.DisplayName;
        }

        [InlineEventHandler]
        private void HandleChangeEmailEvent(UserEmailChangedEvent evnt)
        {
            this.Email = evnt.Email;
        }

        [InlineEventHandler]
        private void HandleAddFriendEvent(FriendAddedEvent evnt)
        {
            this.myFriends.Add(evnt.FriendUserId);
        }

        public void Test()
        {

        }
    }
}
