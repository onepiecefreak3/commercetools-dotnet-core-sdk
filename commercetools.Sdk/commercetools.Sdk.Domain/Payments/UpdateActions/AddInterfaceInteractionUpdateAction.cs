﻿using System.ComponentModel.DataAnnotations;
using commercetools.Sdk.Domain.CustomObject;

namespace commercetools.Sdk.Domain.Payments.UpdateActions
{
    public class AddInterfaceInteractionUpdateAction : UpdateAction<Payment>
    {
        public string Action => "addInterfaceInteraction";

        [Required]
        public ResourceIdentifier Type { get; set; }
        public Fields Fields { get; set; }
    }
}
