using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TP1_ARQWEB.Models
{
    
    public class ModelException : Exception
    {

        public ModelException(string message)
        {
            Message = message;
            UpdateModelState = (model => {  });
        }

        public ModelException(string message, Action<ModelStateDictionary> func)
        {
            Message = message;
            UpdateModelState = func;
        }

        public override string Message { get; }
        public Action<ModelStateDictionary> UpdateModelState { get; set; }

    }

    
}