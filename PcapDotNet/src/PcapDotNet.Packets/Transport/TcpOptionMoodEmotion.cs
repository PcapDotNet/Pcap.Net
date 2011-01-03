namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// An emotion that can be set in a TCP mood option.
    /// </summary>
    public enum TcpOptionMoodEmotion
    {
        /// <summary>
        /// :)  
        /// </summary>
        Happy,      

        /// <summary>
        /// :(  
        /// </summary>
        Sad,  

        /// <summary>
        /// :D  
        /// </summary>
        Amused,  
          
        /// <summary>
        /// %(  
        /// </summary>
        Confused,          

        /// <summary>
        /// :o  
        /// </summary>
        Bored,             

        /// <summary>
        /// :O  
        /// </summary>
        Surprised,         

        /// <summary>
        /// :P  
        /// </summary>
        Silly,             

        /// <summary>
        /// :@  
        /// </summary>
        Frustrated,        

        /// <summary>
        /// >:@ 
        /// </summary>
        Angry,             

        /// <summary>
        /// :|  
        /// </summary>
        Apathetic,         

        /// <summary>
        /// ;)  
        /// </summary>
        Sneaky,            

        /// <summary>
        /// >:) 
        /// </summary>
        Evil,

        /// <summary>
        /// An unknown emotion.
        /// </summary>
        None,
    }
}