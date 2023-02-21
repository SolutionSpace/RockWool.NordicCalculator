using System;
using System.Collections.Generic;
using System.Web.Security;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Newtonsoft.Json;
using REDAirCalculator.Utilities;

namespace REDAirCalculator.Models.DTO
{
    public class ProjectDto
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }

        public ProjectDto(
            Record project,
            Form form,
            MembershipUser user,
            string language
            )
        {
            if (user.ProviderUserKey == null || !user.ProviderUserKey.ToString().Equals(project.MemberKey)) return;

            string projectNameKey = string.Empty;
            foreach (Field field in form.AllFields)
            {
                if (field.Alias != "projectName") continue;
                projectNameKey = field.Id.ToString();
                break;
            }

            Dictionary<string, string> recordDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(project.RecordData);

            string projectName = string.Empty;
            if (recordDataDictionary.ContainsKey(projectNameKey))
            {
                projectName = recordDataDictionary[projectNameKey];
            }

            Guid = project.UniqueId;
            Name = projectName;
            Created = DateTimeLanguageConverter.Convert(project.Created, language).ToString("d'.'M'.'yyyy");
            Updated = DateTimeLanguageConverter.Convert(project.Updated, language).ToString("d'.'M'.'yyyy kl HH:mm");
        }
    }
}