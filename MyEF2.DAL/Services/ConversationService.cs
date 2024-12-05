using Microsoft.EntityFrameworkCore;
using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class ConversationService
    {
        private readonly DatabaseContext _dbContext;
        public ConversationService()
        {
        }
        public ConversationService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<Conversation> GetAll(Organisation organisation)
        {
            return _dbContext.Conversations
                .Include(c => c.Messages)
                .Where(c => c.Organisation.Id == organisation.Id).ToList();
        }
        public Conversation GetConversation(Guid id)
        {
            var conversation = _dbContext.Conversations
                .Include(c => c.Messages)
                .FirstOrDefault(c => c.Id == id)

                ;
            if (conversation != null)
            {
                conversation.Messages = conversation.Messages.OrderBy(m => m.Created).ToList();
            }
            return conversation;
        }
        public void Add(Conversation conversation)
        {
            _dbContext.Conversations.Add(conversation);
            _dbContext.SaveChanges();
        }
        public Conversation Update(Conversation conversation)
        {
            _dbContext.Conversations.Update(conversation);
            _dbContext.SaveChanges();
            return conversation;
        }
        public Conversation Create(Conversation conversation)
        {
            _dbContext.Conversations.Add(conversation);
            _dbContext.SaveChanges();
            return conversation;
        }
    }
}
