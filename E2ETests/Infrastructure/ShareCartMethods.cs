﻿using Microsoft.Playwright;

namespace E2ETests.Infrastructure;

internal static class ShareCartMethods
{
    internal static async Task StartShare(IPage page1, IPage page2, string email1, string email2)
    {
        await page1.FillAsync("#share-cart-email-input", email2);
        await page1.ClickAsync("#add-user-email-btn");
        await page1.ClickAsync("#share-cart-btn");
        await Task.Delay(1000);
        await page2.ClickAsync("#select-join-cart-btn");
        await page2.FillAsync("#cart-owner-email-input", email1);
        await page2.ClickAsync("#join-cart-btn");
    }
}