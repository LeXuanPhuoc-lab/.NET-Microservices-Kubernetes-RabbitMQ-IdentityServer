import NextAuth, { type DefaultSession } from "next-auth";
import { JWT } from "next-auth/jwt";

declare module "next-auth" {
  interface Session {
    user: {
      username: string;
      accessToken: string;
    } & DefaultSession["username"];
  }

  interface Profile {
    username: string;
  }
}

declare module "next-auth/jwt" {
  interface JWT {
    userName: string;
    accessToken: string;
  }
}
