import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

export function middleware(req: NextRequest) {
  if (!req.cookies.has("authjs.session-token")) {
    const redirectRes = NextResponse.redirect(
      new URL(`/api/auth/signin?callBackUrl=${req.nextUrl.pathname}`, req.url)
    );

    redirectRes.headers.set("x-middleware-cache", "no-cache");
    return redirectRes;
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/session"],
  pages: ["/api/auth/signin"],
};
