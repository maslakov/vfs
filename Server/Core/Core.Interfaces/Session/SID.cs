using System;

namespace Core.Sessions
{
    /// <summary>
    /// User descriptor, which is transferred between components
    /// </summary>
    public struct SID
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public string Token;

        /// <summary>
        /// Meaningful name - alias
        /// </summary>
        public string Label;

        /// <summary>
        /// Default empty value
        /// </summary>
        public static readonly SID Empty = new SID { Token = "" };

        public SID(String token)
        {
            Token = token;
            Label = Token;
        }

        public override bool Equals(object obj)
        {
            // compare tokens only
            if (!(obj is SID))
                return false;
            return this.Token == ((SID)obj).Token;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}