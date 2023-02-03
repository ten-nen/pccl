
namespace Pccl.ProjectTemplate.Application.Dtos
{
    public class PagedDto<T>
    {
        public virtual long Total { get; set; }
        public virtual IEnumerable<T> Items { get; set; }
        public PagedDto()
        {

        }
        public PagedDto(IEnumerable<T> items, long total)
        {
            Items = items;
            Total = total;
        }
    }

    public class PagedQueryDto
    {
        public virtual int PageSize { get; set; } = 20;
        public virtual int PageIndex { get; set; } = 1;
        public virtual string? Sorting { get; set; }
    }
}
