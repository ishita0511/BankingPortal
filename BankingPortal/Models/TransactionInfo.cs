//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BankingPortal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TransactionInfo
    {
        public int Id { get; set; }
        public long ReferenceId { get; set; }
        public string TransferMode { get; set; }
        public long ReceiverAccount { get; set; }
        public int Amount { get; set; }
        public long SenderAccount { get; set; }
        public System.DateTime DateofTransaction { get; set; }
        public string Remarks { get; set; }
        public string TransactionType { get; set; }
        public double Balance { get; set; }
    }
}