using System;

namespace Aper_bot.Util
{
    static class ObjectExtensions 
    {
        // Kotlin: fun <T, R> T.let(block: (T) -> R): R
        public static TR Let<T, TR>(this T self, Func<T, TR> block) 
        {
            return block(self);
        }
        
        public static void Let<T>(this T self, Action<T> block) 
        {
            block(self);
        }

        // Kotlin: fun <T> T.also(block: (T) -> Unit): T
        public static T Also<T>(this T self, Action<T> block)
        {
            block(self);
            return self;
        }   
        
        
    }
    
    static class FunctionChaining
    {
        //pipe-forward extension methods
        internal static U Then<T,U>(this T input, Func<T,U> fun) => fun(input);
        internal static void Then<T>(this T input, Action<T> fun) => fun(input);
    }
}