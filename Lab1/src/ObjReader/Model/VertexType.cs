using System;

namespace Lab1.ObjReader.Model
{
    public class VertexType
    {
        private readonly int _minimumVertexLength;
        private readonly string _prefix;

        protected VertexType(int minimumVertexLength, string prefix)
        {
            _minimumVertexLength = minimumVertexLength;
            _prefix = prefix;
        }
        
        public virtual void ProcessData(string[] data)
        {
            if (data.Length < _minimumVertexLength)
                throw new ArgumentException($"Input array must be of minimum length {_minimumVertexLength} data");

            if (!data[0].ToLower().Equals(_prefix))
                throw new ArgumentException($"Data prefix must be '{_prefix}' data");
        }
    }
}