import React from 'react'
import CountDownTimer from './CountDownTimer'
import CarImage from './CarImage'
import { Auction } from '../../../types'
import CurrentBid from './CurrentBid'

type Props = {
    auction: Auction
}

export default function AuctionCard({ auction }: Props) {
    return (
        <a href='#' className='group'>
            <div className='relative w-full bg-gray-200 aspect-video rounded-lg overflow-hidden'>
                <CarImage imageUrl={auction.item.imageUrl} />
                <div className='absolute bottom-2 left-2 text-sm'>
                    <CountDownTimer auctionEnd={auction.auctionEnd} />
                </div>
                <div className='absolute top-2 right-2 text-sm'>
                    <CurrentBid
                        amount={auction.currentHighBid}
                        reservePrice={auction.reservePrice}
                    />
                </div>
            </div>
            <div className='flex justify-between items-center mt-4'>
                <h3 className='text-gray-700'>{auction.item.make} {auction.item.model}</h3>
                <p className='font-semibold text-sm'>{auction.item.year}</p>
            </div>
        </a>
    )
}
