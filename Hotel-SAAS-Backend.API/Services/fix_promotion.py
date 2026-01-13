import os

path = r'c:\Users\tophu\source\repos\Hotel-SAAS-Backend\Hotel-SAAS-Backend.API\Services\PromotionService.cs'

try:
    with open(path, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()

    content = content.replace('"Promotion code already exists"', 'Hotel_SAAS_Backend.API.Models.Constants.Messages.Misc.PromotionExists')
    content = content.replace('"Promotion not found"', 'Hotel_SAAS_Backend.API.Models.Constants.Messages.Misc.PromotionNotFound')
    content = content.replace('"Invalid coupon code"', 'Hotel_SAAS_Backend.API.Models.Constants.Messages.Misc.CouponInvalid')

    with open(path, 'w', encoding='utf-8') as f:
        f.write(content)

    print("Replacement successful")
except Exception as e:
    print(f"Error: {e}")
