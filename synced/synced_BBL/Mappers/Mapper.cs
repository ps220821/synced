using System.Reflection;

namespace velocitaApi.Mappers
{
    public class Mapper
    {
        public static T MapCreate<T>(object dto) where T : class, new()
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "The DTO cannot be null.");
            }

            var target = new T();
            var targetType = typeof(T);
            var sourceType = dto.GetType();

            foreach (var sourceProp in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var targetProp = targetType.GetProperty(sourceProp.Name);
                if (targetProp != null && targetProp.CanWrite &&
                    targetProp.PropertyType == sourceProp.PropertyType)
                {
                    var value = sourceProp.GetValue(dto);
                    targetProp.SetValue(target, value);
                }
            }

            return target;
        }


        public static T MapUpdate<T>(object source, T target) where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "The source object cannot be null.");
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "The target object cannot be null.");
            }

            var sourceType = source.GetType();
            var targetType = target.GetType();

            foreach (var sourceProp in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var targetProp = targetType.GetProperty(sourceProp.Name);
                if (targetProp != null && targetProp.CanWrite &&
                    targetProp.PropertyType == sourceProp.PropertyType)
                {
                    var value = sourceProp.GetValue(source);
                    targetProp.SetValue(target, value);
                }
            }

            return target;
        }
    }
}
