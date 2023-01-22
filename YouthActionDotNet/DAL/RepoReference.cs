using System;
using YouthActionDotNet.Data;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.DAL{
    public class RepositoryReference : IRepositoryReference, IDisposable
    {
        private DBContext context;

        public RepositoryReference(DBContext context)
        {
            this.context = context;
        }
        private GenericRepository<Employee> employeeRepository;
        private GenericRepository<Donor> donorRepository;
        private GenericRepository<Project> projectRepository;
        private GenericRepository<ServiceCenter> serviceCenterRepository;
        private UserRepository userRepository;
        private GenericRepository<Volunteer> volunteerRepository;
        private GenericRepository<VolunteerWork> volunteerWorkRepository;
        private GenericRepository<File> fileRepository;
        public GenericRepository<Employee> EmployeeRepository
        {
            get
            {
                if (this.employeeRepository == null)
                {
                    this.employeeRepository = new GenericRepository<Employee>(context);
                }
                return employeeRepository;
            }
        }

        public GenericRepository<Donor> DonorRepository
        {
            get
            {
                if (this.donorRepository == null)
                {
                    this.donorRepository = new GenericRepository<Donor>(context);
                }
                return donorRepository;
            }
        }

        public GenericRepository<Project> ProjectRepository
        {
            get
            {
                if (this.projectRepository == null)
                {
                    this.projectRepository = new GenericRepository<Project>(context);
                }
                return projectRepository;
            }
        }

        public GenericRepository<ServiceCenter> ServiceCenterRepository
        {
            get
            {
                if (this.serviceCenterRepository == null)
                {
                    this.serviceCenterRepository = new GenericRepository<ServiceCenter>(context);
                }
                return serviceCenterRepository;
            }
        }

        public UserRepository UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new UserRepository(context);
                }
                return userRepository;
            }
        }

        public GenericRepository<Volunteer> VolunteerRepository
        {
            get
            {
                if (this.volunteerRepository == null)
                {
                    this.volunteerRepository = new GenericRepository<Volunteer>(context);
                }
                return volunteerRepository;
            }
        }

        public GenericRepository<VolunteerWork> VolunteerWorkRepository
        {
            get
            {
                if (this.volunteerWorkRepository == null)
                {
                    this.volunteerWorkRepository = new GenericRepository<VolunteerWork>(context);
                }
                return volunteerWorkRepository;
            }
        }

        public GenericRepository<File> FileRepository
        {
            get
            {
                if (this.fileRepository == null)
                {
                    this.fileRepository = new GenericRepository<File>(context);
                }
                return fileRepository;
            }
        }
        
        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }
    }
}