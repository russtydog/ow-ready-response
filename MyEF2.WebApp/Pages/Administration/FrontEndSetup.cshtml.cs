using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;


namespace MyEF2.WebApp.Pages.Administration
{
    [Authorize]
    public class FrontEndSetupModel : PageModel
    {
        private readonly SettingService _settingService;
        private readonly UserService _userService;

        public FrontEndSetupModel(SettingService settingService, UserService userService)
        {
            _settingService = settingService;
            _userService = userService;
        }
        [BindProperty]
        public Setting Setting { get; set; }
        public void OnGet()
        {
            Setting = _settingService.GetSettings();
            var user = _userService.GetUserByAuthId(User.Identity.Name);
            if(!user.IsAdmin)
            {
                RedirectToPage("/Dashboard");
            }
            if(!Setting.UseFrontend){
                RedirectToPage("/Dashboard");
            }           
        }
        public async Task<IActionResult> OnPostAsync()
        {
            
            var setting = _settingService.GetSettings();

            //smtppassword needs to be sent back unencrypted as it auto encrypts again on save
            setting.SMTPPassword = Encryption.Decrypt(setting.SMTPPassword);
            setting.HeroTitle=Setting.HeroTitle;
            setting.HeroDescription=Setting.HeroDescription;
            setting.CustomersStatement=Setting.CustomersStatement;
            setting.Feature1Title=Setting.Feature1Title;
            setting.Feature1Description=Setting.Feature1Description;
            setting.Feature1Highlight1Title=Setting.Feature1Highlight1Title;
            setting.Feature1Highlight1Description=Setting.Feature1Highlight1Description;
            setting.Feature1Hightlight2Title=Setting.Feature1Hightlight2Title;
            setting.Feature1Hightlight2Description=Setting.Feature1Hightlight2Description;
            setting.Feature2Title=Setting.Feature2Title;
            setting.Feature2Description=Setting.Feature2Description;
            setting.Feature2Highlight1Icon=Setting.Feature2Highlight1Icon;
            setting.Feature2Hightlight1Title=Setting.Feature2Hightlight1Title;
            setting.Feature2Hightlight1Description=Setting.Feature2Hightlight1Description;
            setting.Feature2Hightlight2Icon=Setting.Feature2Hightlight2Icon;
            setting.Feature2Hightlight2Title=Setting.Feature2Hightlight2Title;
            setting.Feature2Hightlight2Description=Setting.Feature2Hightlight2Description;
            setting.FeaturesListTitle=Setting.FeaturesListTitle;
            setting.FeaturesListFeature1Icon=Setting.FeaturesListFeature1Icon;
            setting.FeaturesListFeature1Title=Setting.FeaturesListFeature1Title;
            setting.FeaturesListFeature1Description=Setting.FeaturesListFeature1Description;
            setting.FeaturesListFeature2Icon=Setting.FeaturesListFeature2Icon;
            setting.FeaturesListFeature2Title=Setting.FeaturesListFeature2Title;
            setting.FeaturesListFeature2Description=Setting.FeaturesListFeature2Description;
            setting.FeaturesListFeature3Icon=Setting.FeaturesListFeature3Icon;
            setting.FeaturesListFeature3Title=Setting.FeaturesListFeature3Title;
            setting.FeaturesListFeature3Description=Setting.FeaturesListFeature3Description;
            setting.FeaturesListFeature4Icon=Setting.FeaturesListFeature4Icon;
            setting.FeaturesListFeature4Title=Setting.FeaturesListFeature4Title;
            setting.FeaturesListFeature4Description=Setting.FeaturesListFeature4Description;
            setting.FeaturesListFeature5Icon=Setting.FeaturesListFeature5Icon;
            setting.FeaturesListFeature5Title=Setting.FeaturesListFeature5Title;
            setting.FeaturesListFeature5Description=Setting.FeaturesListFeature5Description;
            setting.FeaturesListFeature6Icon=Setting.FeaturesListFeature6Icon;
            setting.FeaturesListFeature6Title=Setting.FeaturesListFeature6Title;
            setting.FeaturesListFeature6Description=Setting.FeaturesListFeature6Description;
            setting.Accordian1Title=Setting.Accordian1Title;
            setting.Accordian1Description=Setting.Accordian1Description;
            setting.Accordian2Title=Setting.Accordian2Title;
            setting.Accordian2Description=Setting.Accordian2Description;
            setting.Accordian3Title=Setting.Accordian3Title;
            setting.Accordian3Description=Setting.Accordian3Description;
            setting.AccordianHeroTitle=Setting.AccordianHeroTitle;
            setting.AccordianHeroDescription1=Setting.AccordianHeroDescription1;
            setting.AccordianHeroDescription2=Setting.AccordianHeroDescription2;
            setting.EnableReviews=Setting.EnableReviews;
            setting.ReviewsTitle=Setting.ReviewsTitle;
            setting.Review1Rating = Setting.Review1Rating;
            setting.Review1Title=Setting.Review1Title;
            setting.Review1Description=Setting.Review1Description;
            setting.Review1Author=Setting.Review1Author;
            setting.Review1AuthorCompany=Setting.Review1AuthorCompany;
            setting.Review2Rating=Setting.Review2Rating;
            setting.Review2Title=Setting.Review2Title;
            setting.Review2Description=Setting.Review2Description;
            setting.Review2Author = Setting.Review2Author;
            setting.Review2AuthorCompany = Setting.Review2AuthorCompany;
            setting.Review3Rating = Setting.Review3Rating;
            setting.Review3Title = Setting.Review3Title;
            setting.Review3Description = Setting.Review3Description;
            setting.Review3Author = Setting.Review3Author;
            setting.Review3AuthorCompany = Setting.Review3AuthorCompany;
            setting.Review4Rating = Setting.Review4Rating;
            setting.Review4Title = Setting.Review4Title;
            setting.Review4Description = Setting.Review4Description;
            setting.Review4Author = Setting.Review4Author;
            setting.Review4AuthorCompany = Setting.Review4AuthorCompany;
            setting.DownloadActionTitle = Setting.DownloadActionTitle;
            setting.DownloadActionDescription = Setting.DownloadActionDescription;
            setting.SubscribeText = Setting.SubscribeText;

            _settingService.UpdateSettings(setting,User.Identity.Name);
            return RedirectToPage("/Dashboard");
        }
    }
}