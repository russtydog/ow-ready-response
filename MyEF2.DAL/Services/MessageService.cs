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
    public class MessageService
    {
        private readonly DatabaseContext _dbContext;

        public MessageService()
        {
        }
        public MessageService(DatabaseContext dbContext)
        {
            _dbContext=dbContext;
        }
        //public MessageService(DatabaseContext dbContext)
        //{
        //    _dbContext = dbContext;
        //}
        public List<Message> GetMessages()
        {
            return _dbContext.Messages.ToList();
        }
        public Message GetMessage(Guid id)
        {
            return _dbContext.Messages.Find(id);
        }
        public Message Create(Message message)
        {
            var existingConversation = _dbContext.Conversations
            .Include(c => c.Organisation)
            .FirstOrDefault(c => c.Id == message.Conversation.Id);

            // Replace message.Conversation with existingConversation
            message.Conversation = existingConversation;


            //create the Message
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();
            return message;
        }
        public Message Update(Message message)
        {
            //update the message
            _dbContext.Messages.Update(message);
            _dbContext.SaveChanges();
            return message;
        }
    }
}
