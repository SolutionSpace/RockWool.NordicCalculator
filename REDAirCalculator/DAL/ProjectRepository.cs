using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using REDAirCalculator.Models.DTO;
using REDAirCalculator.Utilities;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;

namespace REDAirCalculator.DAL
{
    public interface IProjectRepository
    {
        void Save(string entryGuid, string formGuid, int memberId);
        List<ProjectDto> GetAll(Guid guid, string language);
        Form Get(Guid guid);
        void Update(Record entry, Form form);
        void Delete(Record entry, Form form);
    }

    public class ProjectRepository : IProjectRepository
    {
        private readonly IRecordStorage _recordStorage;

        public ProjectRepository(IRecordStorage recordStorage)
        {
            _recordStorage = recordStorage;
        }

        public void Save(string entryGuid, string formGuid, int memberId)
        {
            Form form = Get(new Guid(formGuid));
            Record entry = FormHelper.GetEntry(new Guid(entryGuid), form);

            entry.MemberKey = memberId.ToString();

            Update(entry, form);
        }

        public List<ProjectDto> GetAll(Guid guid, string language)
        {
            Form form = FormHelper.GetForm(guid);
            IEnumerable<Record> formEntries = _recordStorage.GetAllRecords(form, false);
            MembershipUser user = Membership.GetUser();

            return formEntries
                .Select(project => new ProjectDto(project, form, user, language))
                .Where(project => !string.IsNullOrEmpty(project.Name))
                .ToList();
        }

        public Form Get(Guid guid)
        {
            return FormHelper.GetForm(guid);
        }

        public void Update(Record entry, Form form)
        {
            _recordStorage.UpdateRecord(entry, form);
        }

        public void Delete(Record entry, Form form)
        {
            _recordStorage.DeleteRecord(entry, form);
        }
    }
}