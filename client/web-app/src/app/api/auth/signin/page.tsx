import EmptyFilter from '@/app/components/EmptyFilter'
import React from 'react'

export default async function SignIn(
    { searchParams }: {
        searchParams: Promise<{ callBackUrl: string }>
    }) {
    const { callBackUrl } = await searchParams;

    return (
        <EmptyFilter
            title='You need to be logged in to do that'
            subTitle='Please click below to login'
            showLogin
            callBackUrl={callBackUrl}
        />
    )
}
