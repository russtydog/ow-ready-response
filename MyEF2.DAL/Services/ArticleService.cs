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
    public class ArticleService
    {
        private readonly DatabaseContext _dbContext;

        public ArticleService()
        {
        }
        public ArticleService(DatabaseContext dbContext)
        {
            _dbContext=dbContext;
        }
        
        public List<Article> GetArticles(Organisation organisation,string UserName)
        {
          return _dbContext.Articles.Where(x=>x.Organisation==organisation).ToList();
        }
        public Article GetArticle(Guid id,string UserName)
        {
            Article article = _dbContext.Articles.Where(x=>x.Id==id).FirstOrDefault();

            AuditService auditService = new AuditService(_dbContext);
            auditService.CreateAuditFromObject(article, null, "Articles", article.Id.ToString(), article.Organisation, UserName, "View");

            return article;
        }
        public Article Create(Article Article,string UserName)
        {
            

            //create the Article
            _dbContext.Articles.Add(Article);
            _dbContext.SaveChanges();

            AuditService auditService = new AuditService(_dbContext);
            auditService.CreateAuditFromObject(Article, null, "Articles", Article.Id.ToString(), Article.Organisation, UserName, "Create");

            return Article;
        }
        public Article Update(Article Article,string UserName)
        {
            Article originalArticle = _dbContext.Articles.AsNoTracking().Where(x=>x.Id==Article.Id).FirstOrDefault();

            //update the Article
            _dbContext.Articles.Update(Article);
            _dbContext.SaveChanges();

            AuditService auditService = new AuditService(_dbContext);
            auditService.CreateAuditFromObject(Article, originalArticle, "Articles", Article.Id.ToString(),Article.Organisation, UserName, "Update");

            return Article;
        }
        public string Delete(Guid Id, string UserName)
        {
            try
            {
                Article originalArticle = _dbContext.Articles.AsNoTracking().Where(x=>x.Id==Id).FirstOrDefault();
                Article article = _dbContext.Articles.Where(x=>x.Id==Id).FirstOrDefault();

                

                AuditService auditService = new AuditService(_dbContext);
                auditService.CreateAuditFromObject(originalArticle, null, "Articles", originalArticle.Id.ToString(), originalArticle.Organisation, UserName, "Delete");

                _dbContext.Articles.Remove(article);
                _dbContext.SaveChanges();
                return "Successfully Deleted Product";
            }
            catch(Exception ex)
            {
                return ex.Message;   
            }
        }
    }
}
